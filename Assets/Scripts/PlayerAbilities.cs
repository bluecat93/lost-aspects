using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public EnemiesList EnemiesList;
    public float minAttackDistance = 1.0f;
    private PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        playerStats = transform.gameObject.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
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
        float closestViableEnemyDistance = minAttackDistance;
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
