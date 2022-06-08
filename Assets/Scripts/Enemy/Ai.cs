using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;
using Animation;
using Mirror;

namespace Enemy
{
    public class Ai : NetworkBehaviour
    {
        //state machine:
        private enum State
        {
            Idle,
            Roaming,
            Chasing,
            Dead,
            Attacking,
            Reseting
        }

        #region parameters

        private Vector3 startingPosition;
        private Vector3 roamPosition;
        [SyncVar] private State state;

        private float currentDeathTimer;
        private bool deadOnlyOnce = false;
        bool isAttacking = false;
        private int currentAttack;

        //events
        //gets int currentAttack - the current number of attack, 
        //bool isStartAttack - true if its the start of the attack animation or false if its the end of the attack animation.
        [HideInInspector] public UnityEvent<int, bool> attackEvent;

        private AiMovement _aiMovement;

        private AiMovement AiMvmnt
        {
            get
            {
                if (this._aiMovement == null)
                    this._aiMovement = GetComponent<AiMovement>();

                return this._aiMovement;
            }
        }

        private Animator _animator;

        private Animator Anmtr
        {
            get
            {
                if (this._animator == null)
                    this._animator = GetComponentInChildren<Animator>();

                return this._animator;
            }
        }

        private Stats _enemyStats;

        private Stats EnmyStts
        {
            get
            {
                if (this._enemyStats == null)
                    this._enemyStats = GetComponent<Stats>();

                return this._enemyStats;
            }
        }

        private EventsHandler _eventHandler;

        private EventsHandler EvntHndlr
        {
            get
            {
                if (this._eventHandler == null)
                    this._eventHandler = GetComponentInChildren<EventsHandler>();
                return this._eventHandler;
            }
        }

        [SyncVar] private GameObject Target = null;

        #endregion
        private void Awake()
        {
            this.state = State.Roaming;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.EvntHndlr.OnAttackEventTrigger.AddListener(AttackEvent);
            this.startingPosition = transform.position;
            this.roamPosition = GetRoamingPosition();
            currentAttack = 1;
        }

        // Update is called once per frame
        void Update()
        {
            // if (isClient)
            // {
            //     Debug.Log("Running on client!!!");
            // }

            if (isServer)
            {
                ServerUpdate();
            }

        }

        [Server]
        void ServerUpdate()
        {
            // Debug.Log("Running on server!!!");
            isAttacking = this.Anmtr.GetCurrentAnimatorStateInfo(0).IsName(Finals.AttackingAnimation);
            this.Anmtr.SetInteger(Finals.StateAnimation, (int)state);

            HandleDeath();
            HandleStateMachine();
        }

        private void HandleStateMachine()
        {
            switch (this.state)
            {
                case State.Roaming:
                    this.AiMvmnt.MoveTo(this.roamPosition);
                    if (AiMvmnt.ReachedPosition())
                    {
                        //Reached roam position
                        this.roamPosition = GetRoamingPosition();
                    }
                    FindTarget();
                    break;
                case State.Attacking:
                    break;
                case State.Chasing:
                    this.AiMvmnt.MoveTo(Target.transform.position);
                    AttackTarget();
                    StopChasing();
                    break;

                case State.Dead:
                    this.AiMvmnt.MoveTo(transform.position);
                    if (this.currentDeathTimer <= Time.time)
                    {
                        RpcKillEnemy();
                    }
                    break;

                case State.Reseting:
                    this.Target = null;
                    this.AiMvmnt.MoveTo(this.startingPosition);
                    this.EnmyStts.Heal(this.EnmyStts.GetMaxHealth());
                    if (this.AiMvmnt.ReachedPosition())
                    {
                        this.state = State.Roaming;
                    }
                    break;
            }
        }

        [ClientRpc]
        private void RpcKillEnemy()
        {
            // TODO maybe destory this later instead of inactive.
            transform.gameObject.SetActive(false);
        }
        private void HandleDeath()
        {
            if (!this.EnmyStts.isEnemyAlive())
            {
                this.state = State.Dead;
                if (!this.deadOnlyOnce)
                {
                    this.currentDeathTimer = Time.time + this.EnmyStts.GetDeathTimer();
                }
                this.deadOnlyOnce = true;
            }
        }

        private Vector3 GetRoamingPosition()
        {
            NavMeshHit navMeshHit;
            float randomRange;
            Vector3 randomDirection;

            randomRange = UnityEngine.Random.Range(4f, 7f);
            randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
            if (NavMesh.SamplePosition(transform.position + (randomDirection * randomRange), out navMeshHit, randomRange, NavMesh.AllAreas))
            {
                // Debug.Log("navMesh position: " + navMeshHit.position + "\nrandom position: " + randomDirection*randomRange);
                return navMeshHit.position;
            }
            return transform.position;
        }

        private void FindTarget()
        {
            // get all colliders in range.
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, EnmyStts.GetSightDistance());
            // running on all colliders in range
            foreach (Collider hitCollider in hitColliders)
            {
                // found a target
                if (hitCollider.gameObject.CompareTag(Finals.PlayerTag))
                {
                    // found only one target
                    if (Target == null)
                    {
                        Target = hitCollider.gameObject;
                    }
                    // found multiple targets, choosing the closest.
                    else
                    {
                        if (Vector3.Distance(this.transform.position, Target.transform.position) >
                        Vector3.Distance(this.transform.position, hitCollider.gameObject.transform.position))
                        {
                            Target = hitCollider.gameObject;
                        }
                    }
                }
            }

            if (Target != null)
            {
                this.state = State.Chasing;
            }
        }

        private void AttackTarget()
        {
            // rotate towords player
            LookAtObject(Target);
            if (Vector3.Distance(transform.position, Target.transform.position) <= this.EnmyStts.GetAttackDistance())
            {
                this.Anmtr.SetBool("InAttackRange", true);
                this.AiMvmnt.StopMoving();
                // used for animations only
                this.state = State.Attacking;
                this.Anmtr.SetInteger("AttackNumber", this.currentAttack);
                this.Anmtr.SetInteger("State", (int)this.state);
                this.state = State.Chasing;

            }
            else
            {
                this.Anmtr.SetBool("InAttackRange", false);
            }
        }

        private void StopChasing()
        {
            if (Vector3.Distance(transform.position, Target.transform.position) > this.EnmyStts.GetMaxFollowDistance())
            {
                this.state = State.Reseting;
            }
        }

        public void LookAtObject(GameObject gameObject)
        {
            transform.LookAt(gameObject.transform);
        }

        public Player.Stats getPlayerStats()
        {
            if (this.Target == null)
                return null;
            return this.Target.GetComponent<Player.Stats>();
        }

        public void AttackEvent(bool isAttacking)
        {
            this.attackEvent.Invoke(currentAttack, isAttacking);
            // finished attack animation.
            if (!isAttacking)
            {
                this.currentAttack = (this.currentAttack == this.EnmyStts.GetNumberOfAttacks()) ? 1 : this.currentAttack + 1;
            }
        }

        public GameObject GetAttackingPlayer()
        {
            if (this.Target == null)
                return null;
            return Target;
        }

    }

}
