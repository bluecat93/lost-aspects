using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public EnemiesList EnemiesList;
    public float maxAttackDistance = 1.0f;
    private PlayerStats playerStats;

    private RaycastHit raycastHit;
    private Ray[] ray;
    public float playerMiddleHeight = 2;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = transform.gameObject.GetComponent<PlayerStats>();
        ray = new Ray[3];
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: add the content of this for loop to the for loop inside the raycast
        // when you no longer want to see the raycast at all times.
        // 3 rays that indicate the attack range.
        for(int i=-1;i<=1;i++)
        {
            ray[i + 1] = new Ray(transform.position + Vector3.up * playerMiddleHeight,
                (transform.forward + (i*transform.right)).normalized * maxAttackDistance);
            Debug.DrawRay(ray[i+1].origin, ray[i+1].direction * maxAttackDistance);
        }

        float attacking = Input.GetAxisRaw("Swing");
        if (attacking != 0)
        {
            for(int i = 0; i <= 2; i++)
            {
                if (Physics.Raycast(ray[i], out raycastHit, maxAttackDistance))
                {
                    Transform hit = checkHit();
                    if( hit != null)
                    {
                        // Enemy found.
                        EnemyAi enemyAi = hit.gameObject.GetComponent<EnemyAi>();
                        enemyAi.TakeDamage(playerStats.damage);
                    }
                }
            }

            // Player attacked
            //Transform hit = checkHitDeprecated();
            /*if (hit != null)
            {
                // Enemy found.
                EnemyAi enemyAi = hit.gameObject.GetComponent<EnemyAi>();
                enemyAi.TakeDamage(playerStats.damage);
            }*/
        }
    }

    // still saved this method. maybe someone will want to see how to find the closest object from a given list of objects.
    [Obsolete("This method is deprecated, please use checkHit instead.")]
    private Transform checkHitDeprecated()
    {
        Transform closestViableEnemy = null;
        float closestViableEnemyDistance = maxAttackDistance;
        foreach (Transform enemy in EnemiesList.getEnemies())
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (closestViableEnemyDistance >= distance)
            {
                closestViableEnemyDistance = distance;
                closestViableEnemy = enemy;
            }
        }
        return closestViableEnemy;
    }

    private Transform checkHit()
    {
        Debug.Log("Raycast hit: " + raycastHit.collider.gameObject.name);
        foreach(Transform enemy in EnemiesList.getEnemies())
        {
            if (raycastHit.collider.gameObject == enemy.gameObject)
            {
                return enemy;
            }
        }
        return null;
    }
}
