using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public EnemiesList EnemiesList;
    public float maxAttackDistance = 1.0f;
    private PlayerStats playerStats;
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
        // 3 rays that indicate the attack range.
        for(int i=-1;i<=1;i++)
        {
            ray[i + 1] = new Ray(transform.position + Vector3.up * playerMiddleHeight, (transform.forward + (i*transform.right)).normalized * maxAttackDistance);
            Debug.DrawRay(ray[i+1].origin, ray[i+1].direction * maxAttackDistance);
        }

        float attacking = Input.GetAxisRaw("Swing");
        if (attacking != 0)
        {
            // Player attacked
            Transform hit = checkHit();
            if (hit != null)
            {
                // Enemy found.
                EnemyAi enemyAi = hit.gameObject.GetComponent<EnemyAi>();
                enemyAi.TakeDamage(playerStats.damage);
            }
        }
    }

    private Transform checkHit()
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
}
