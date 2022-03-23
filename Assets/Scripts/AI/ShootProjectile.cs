using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    private EnemyAi enemyAi;
    [SerializeField] private Transform fireBall;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        enemyAi = GetComponent<EnemyAi>();
        enemyAi.OnEndAttackAnimation += EnemyAi_OnEndAttackAnimation;
    }

    private void EnemyAi_OnEndAttackAnimation(object sender, System.EventArgs e)
    {
        Vector3 fireballPositionYOffset = new Vector3(0, 1.201f, 0);
        Vector3 fireballPositionXZOffset = transform.forward * 1.2f;
        Instantiate(fireBall, this.transform.position + fireballPositionYOffset + fireballPositionXZOffset, Quaternion.identity);
    }

}
