using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private enum State
    {
        Roaming,
        Chasing,
        Dead,
        //avoids animation canceling
        Attacking,
        Reseting
    }
    //private AttackAnimations attackAnimations;
    private AiMovement aiMovement;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private State state;
    private float nextAttackTime;

    public EnemiesList enemiesList;

    public int maxHealth = 100;
    private int currentHealth;
    private float invulnerabilityFrame;
    public float invulnerabilityFrameAmount = 0.5f;

    public float attackTimeIntervals = 1f;
    public int attackDamage = 5;

    public float maxFollowDistance = 20f;
    public float sightDistance = 10f;
    public float attackDistance = 5f;

    //TODO: might want to have a script that gives me all of the things i need like position and stats.
    //Player stuff 
    public GameObject player;
    public PlayerStats playerStats;



    private void Awake()
    {
        //attackAnimations = GetComponent<AttackAnimations>();
        aiMovement = GetComponent<AiMovement>();
        state = State.Roaming;
    }

    // Start is called before the first frame update
    void Start()
    {
        enemiesList.addEnemy(transform);
        startingPosition = transform.position;
        roamPosition = GetRoamingPosition();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
            state = State.Dead;
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
                enemiesList.removeEnemy(transform);
                transform.gameObject.SetActive(false);
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
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f),0 ,UnityEngine.Random.Range(-1f, 1f)).normalized;
        return startingPosition + randomDirection * Random.Range(10f, 30f);
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
        if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
        {
            aiMovement.StopMoving();
            if(Time.time > nextAttackTime)
            {
                nextAttackTime = Time.time + attackTimeIntervals;
                playerStats.TakeDamage(attackDamage);
                state = State.Attacking;
                //do animation
                state = State.Chasing;
            }
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

}
