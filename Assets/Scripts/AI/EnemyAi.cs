using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

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

    private AiMovement aiMovement;
    private Animator animator;
    private EnemyStats stats;

    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private State state;

    private float nextAttackTime;
    private bool isAttacking;
    private bool attackOnlyOnce = false;
    private float currentDeathTimer;
    private bool deadOnlyOnce = false;

    // this is an event to let other scripts know this is attacking time.
    public event EventHandler OnAttacking;


    //TODO: Will be refactored when we have multiplayer.
    //Player stuff 
    public GameObject player;
    private PlayerStats playerStats;


    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        aiMovement = GetComponent<AiMovement>();
        state = State.Roaming;
        animator = GetComponentInChildren<Animator>();
        playerStats = player.GetComponent<PlayerStats>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");

        animator.SetInteger("State", (int)state);
        if (!stats.isEnemyAlive())
        {
            state = State.Dead;
            if (!deadOnlyOnce)
            {
                currentDeathTimer = Time.time + stats.getDeathTimer();
            }
            deadOnlyOnce = true;
        }
        switch (state)
        {
            case State.Roaming:
                aiMovement.MoveTo(roamPosition);
                if (aiMovement.ReachedPosition())
                {
                    //Reached roam position
                    roamPosition = GetRoamingPosition();
                }
                FindTarget();
                break;

            case State.Chasing:
                aiMovement.MoveTo(player.transform.position);
                AttackTarget();
                StopChasing();
                break;

            case State.Dead:
                aiMovement.MoveTo(transform.position);
                if (currentDeathTimer <= Time.time)
                {
                    Destroy(gameObject);
                }
                break;

            case State.Attacking:
                break;

            case State.Reseting:
                aiMovement.MoveTo(startingPosition);
                if (aiMovement.ReachedPosition())
                {
                    state = State.Roaming;
                }
                break;

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
        if (Vector3.Distance(transform.position, player.transform.position) <= stats.getSightDistance())
        {
            state = State.Chasing;
        }
    }

    private void AttackTarget()
    {
        // rotate towords player
        LookAtObject(player);
        if (Vector3.Distance(transform.position, player.transform.position) <= stats.getAttackDistance())
        {
            animator.SetBool("InAttackRange", true);
            aiMovement.StopMoving();
            if (Time.time > nextAttackTime)
            {
                attackOnlyOnce = true;
                nextAttackTime = Time.time + stats.getAttackTimeIntervals();
                state = State.Attacking;
                // TODO: change this to a more reasonable location (after the animation finish).
                OnAttacking?.Invoke(this, EventArgs.Empty);
                // do animation
                animator.SetInteger("State", (int)state);
                state = State.Chasing;
            }
        }
        else
        {
            animator.SetBool("InAttackRange", false);
        }
    }

    private void StopChasing()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > stats.getMaxFollowDistance())
        {
            state = State.Reseting;
        }
    }



    public void LookAtObject(GameObject gameObject)
    {
        transform.LookAt(gameObject.transform);
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    public bool GetAttackOnlyOnce()
    {
        return attackOnlyOnce;
    }

    public void SetAttackOnlyOnce(bool attackOnlyOnce)
    {
        this.attackOnlyOnce = attackOnlyOnce;
    }

    public PlayerStats GetPlayerStats()
    {
        return this.playerStats;
    }

}
