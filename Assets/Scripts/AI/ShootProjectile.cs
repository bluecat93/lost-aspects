using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    private EnemyAi enemyAi;
    //This is the prefab for the projectile
    [SerializeField] private Transform projectilePrefab;
    [SerializeField] private float xzOffset = 1.2f;
    [SerializeField] private float yOffset = 1.201f;


    // Awake is called when the script instance is being loaded
    void Awake()
    {
        enemyAi = GetComponent<EnemyAi>();
        enemyAi.OnEndAttackAnimation += EnemyAi_OnEndAttackAnimation;
    }

    private bool EnemyAi_OnEndAttackAnimation(bool attackOnlyOnce, Transform target)
    {
        if (attackOnlyOnce)
        {

            Transform projectileInstance;
            Vector3 fireballPositionYOffset = new Vector3(0, yOffset, 0);
            Vector3 fireballPositionXZOffset = transform.forward * xzOffset;
            Vector3 fireballFinalPosition = this.transform.position + fireballPositionYOffset + fireballPositionXZOffset;
            projectileInstance = Instantiate(projectilePrefab, fireballFinalPosition, Quaternion.identity);
            projectileInstance.gameObject.SetActive(true);
            projectileInstance.GetComponent<FireballPhysics>().Setup(target);

        }
        return false;
    }

}
