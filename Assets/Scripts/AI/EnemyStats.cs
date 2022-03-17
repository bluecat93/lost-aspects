using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;
    [SerializeField]
    private int attackDamage = 5;
    [SerializeField]
    private float invulnerabilityFrameAmount = 0.5f;
    [SerializeField]
    private float attackTimeIntervals = 1f;
    [SerializeField]
    private float maxFollowDistance = 20f;
    [SerializeField]
    private float sightDistance = 10f;
    [SerializeField]
    private float attackDistance = 5f;
    [SerializeField]
    private float deathTimer = 5f;

    private int currentHealth;
    private float invulnerabilityFrame = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public bool isEnemyAlive()
    {
        return currentHealth >= 0;
    }
    public void TakeDamage(int damage)
    {
        if (invulnerabilityFrame < Time.time)
        {
            invulnerabilityFrame = Time.time + invulnerabilityFrameAmount;
            currentHealth -= damage;
        }
    }
    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public float getDeathTimer()
    {
        return this.deathTimer;
    }
    public float getMaxFollowDistance()
    {
        return this.maxFollowDistance;
    }
    public float getSightDistance()
    {
        return this.sightDistance;
    }
    public float getAttackDistance()
    {
        return this.attackDistance;
    }
    public float getAttackTimeIntervals()
    {
        return this.attackTimeIntervals;
    }
    public int getAttackDamage()
    {
        return this.attackDamage;
    }


}