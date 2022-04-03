using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    private enum State
    {
        Idle,
        Roaming,
        Chasing,
        Dead,
        //avoids animation canceling
        Attacking,
        Reseting
    }


    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private State state;

    private float nextAttackTime;
    private bool isAttacking;
    private bool attackOnlyOnce = false;
    private float currentDeathTimer;
    private bool deadOnlyOnce = false;


    //TODO: Will be refactored when we have multiplayer.
    //Player stuff 
    public GameObject player;

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

    private EnemyStats _enemyStats;

    private EnemyStats EnmyStts
    {
        get
        {
            if (this._enemyStats == null)
                this._enemyStats = GetComponent<EnemyStats>();

            return this._enemyStats;
        }
    }

    private void Awake()
    {
        this.state = State.Roaming;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.startingPosition = transform.position;
        this.roamPosition = GetRoamingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        this.isAttacking = this.Anmtr.GetCurrentAnimatorStateInfo(0).IsName("Attacking");

        this.Anmtr.SetInteger("State", (int)state);
        if (!this.EnmyStts.isEnemyAlive())
        {
            this.state = State.Dead;
            if (!deadOnlyOnce)
            {
                this.currentDeathTimer = Time.time + this.EnmyStts.getDeathTimer();
            }
            this.deadOnlyOnce = true;
        }
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

            case State.Attacking:
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


    private Vector3 GetRoamingPosition()
    {
        NavMeshHit navMeshHit;
        float randomRange;
        Vector3 randomDirection;

        randomRange = Random.Range(4f, 7f);
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
            if (Time.time > nextAttackTime)
            {
                this.attackOnlyOnce = true;
                this.nextAttackTime = Time.time + this.EnmyStts.getAttackTimeIntervals();
                this.state = State.Attacking;
                // do animation
                this.Anmtr.SetInteger("State", (int)this.state);
                this.state = State.Chasing;
            }
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

    public bool getIsAttacking()
    {
        return this.isAttacking;
    }

    public bool getAttackOnlyOnce()
    {
        return this.attackOnlyOnce;
    }

    public void setAttackOnlyOnce(bool attackOnlyOnce)
    {
        this.attackOnlyOnce = attackOnlyOnce;
    }

    public PlayerStats getPlayerStats()
    {
        return this.PlyrStts;
    }
}
