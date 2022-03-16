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
    //private AttackAnimations attackAnimations;
    private AiMovement aiMovement;
    private Animator animator;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private State state;
    private float nextAttackTime;
    public bool isAttacking;

    public int maxHealth = 100;
    private int currentHealth;
    private float invulnerabilityFrame;
    public float invulnerabilityFrameAmount = 0.5f;

    public float attackTimeIntervals = 1f;
    public int attackDamage = 5;
    public bool attackOnlyOnce = false;

    public float maxFollowDistance = 20f;
    public float sightDistance = 10f;
    public float attackDistance = 5f;

    public float deathTimer = 5f;
    private float currentDeathTimer;
    private bool deadOnlyOnce = false;


    //TODO: might want to have a script that gives me all of the things i need like position and stats.
    //Player stuff 
    public GameObject player;
    public PlayerStats playerStats;





    private void Awake()
    {
        aiMovement = GetComponent<AiMovement>();
        state = State.Roaming;
        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");

        animator.SetInteger("State", (int)state);
        if (currentHealth <= 0)
        {
            state = State.Dead;
            if(!deadOnlyOnce)
            {
                currentDeathTimer = Time.time + deathTimer;
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
                    transform.gameObject.SetActive(false);
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

        randomRange = Random.Range(4f, 7f);
        randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        if (NavMesh.SamplePosition(transform.position+(randomDirection*randomRange), out navMeshHit, randomRange, NavMesh.AllAreas))
        {
            // Debug.Log("navMesh position: " + navMeshHit.position + "\nrandom position: " + randomDirection*randomRange);
            return navMeshHit.position;
        }
        return transform.position;
    }

    private void FindTarget()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= sightDistance)
        {
            state = State.Chasing;
        }
    }

    private void AttackTarget()
    {
        // rotate towords player
        LookAtObject(player);
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            animator.SetBool("InAttackRange", true);
            aiMovement.StopMoving();
            if(Time.time > nextAttackTime)
            {
                attackOnlyOnce = true;
                nextAttackTime = Time.time + attackTimeIntervals;
                state = State.Attacking;
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
        if (Vector3.Distance(transform.position, player.transform.position) > maxFollowDistance)
        {
            state = State.Reseting;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invulnerabilityFrame < Time.time)
        {
            invulnerabilityFrame = Time.time + invulnerabilityFrameAmount;
            currentHealth -= damage;
        }
    }

    public void LookAtObject(GameObject gameObject)
    {
        transform.LookAt(gameObject.transform);
    }

}
