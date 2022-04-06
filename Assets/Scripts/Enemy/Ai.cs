using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;


namespace Enemy
{
    public class Ai : MonoBehaviour
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
        private State state;

        private float nextAttackTime;
        private bool attackOnlyOnce = false;
        private float currentDeathTimer;
        private bool deadOnlyOnce = false;
        bool isAttacking = false;
        private int currentAttack;

        //events
        //gets int currentAttack - the current number of attack, 
        //bool isStartAttack - true if its the start of the attack animation or false if its the end of the attack animation.
        [HideInInspector] public UnityEvent<int, bool> attackEvent;


        //Player stuff
        private GameObject player;

        private PlayerStats _playerStats;

        private PlayerStats PlyrStts
        {
            get
            {
                if (this._playerStats == null)
                    this._playerStats = player.GetComponent<PlayerStats>();

                return this._playerStats;
            }
        }


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

        private AnimationEventsHandler _eventHandler;

        private AnimationEventsHandler EvntHndlr
        {
            get
            {
                if (this._eventHandler == null)
                    this._eventHandler = GetComponentInChildren<AnimationEventsHandler>();
                return this._eventHandler;
            }
        }

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
            this.player = GameObject.FindGameObjectWithTag("Player");
            currentAttack = 1;
        }

        // Update is called once per frame
        void Update()
        {
            isAttacking = this.Anmtr.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
            this.Anmtr.SetInteger("State", (int)state);

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
                    this.AiMvmnt.MoveTo(player.transform.position);
                    AttackTarget();
                    StopChasing();
                    break;

                case State.Dead:
                    this.AiMvmnt.MoveTo(transform.position);
                    if (this.currentDeathTimer <= Time.time)
                    {
                        transform.gameObject.SetActive(false);
                    }
                    break;

                case State.Reseting:
                    this.AiMvmnt.MoveTo(this.startingPosition);
                    if (this.AiMvmnt.ReachedPosition())
                    {
                        this.state = State.Roaming;
                    }
                    break;
            }
        }

        private void HandleDeath()
        {
            if (!this.EnmyStts.isEnemyAlive())
            {
                this.state = State.Dead;
                if (!this.deadOnlyOnce)
                {
                    this.currentDeathTimer = Time.time + this.EnmyStts.getDeathTimer();
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
            if (Vector3.Distance(transform.position, player.transform.position) <= this.EnmyStts.getSightDistance())
            {
                this.state = State.Chasing;
            }
        }

        private void AttackTarget()
        {
            // rotate towords player
            LookAtObject(player);
            if (Vector3.Distance(transform.position, player.transform.position) <= this.EnmyStts.getAttackDistance())
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
            if (Vector3.Distance(transform.position, player.transform.position) > this.EnmyStts.getMaxFollowDistance())
            {
                this.state = State.Reseting;
            }
        }

        public void LookAtObject(GameObject gameObject)
        {
            transform.LookAt(gameObject.transform);
        }

        public PlayerStats getPlayerStats()
        {
            return this.PlyrStts;
        }

        public void AttackEvent(bool isAttacking)
        {
            this.attackEvent.Invoke(currentAttack, isAttacking);
            // finished attack animation.
            if (!isAttacking)
            {
                this.currentAttack = (this.currentAttack == this.EnmyStts.getNumberOfAttacks()) ? 1 : this.currentAttack + 1;
            }
        }

    }

}
