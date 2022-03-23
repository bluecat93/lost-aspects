using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    private EnemyAi enemyAi;
    [SerializeField] private Transform fireBall;
    [SerializeField] private float xzOffset = 1.2f;
    [SerializeField] private float yOffset = 1.201f;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        enemyAi = GetComponent<EnemyAi>();
        enemyAi.OnEndAttackAnimation += EnemyAi_OnEndAttackAnimation;
    }

    private bool EnemyAi_OnEndAttackAnimation(bool attackOnlyOnce)
    {
        if (attackOnlyOnce)
        {
            Vector3 fireballPositionYOffset = new Vector3(0, yOffset, 0);
            Vector3 fireballPositionXZOffset = transform.forward * xzOffset;
            Vector3 fireballFinalPosition = this.transform.position + fireballPositionYOffset + fireballPositionXZOffset;
            fireBall.GetComponent<FireballPhysics>().Setup(fireballFinalPosition);
            Instantiate(fireBall, fireballFinalPosition, Quaternion.identity);
        }
        return false;
    }

}
