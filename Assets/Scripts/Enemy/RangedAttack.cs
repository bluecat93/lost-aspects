using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Class <c>RangedAttack</c> create a ranged attack for the enemy ai. note: place this component on an empty object that is the location to shoot from
    /// </summary>    
    public class RangedAttack : Attack
    {
        [Header("Ranged Attack")]
        [Tooltip("The prefab of the projectile that is being summoned.")]
        [SerializeField] private GameObject projectilePrefab;
        [Tooltip("The spawned projectile's speed")]
        [SerializeField] private float projectileSpeed;

        void Start()
        {
            EnmyAi.attackEvent.AddListener(AttackEvent);
        }
        public override void AttackEvent(int attackNumber, bool isStartAttack)
        {
            if (attackNumber == this.attackNumber)
            {
                if (isStartAttack)
                {
                    // start of the attack animation, maybe add particles option later here.
                }
                else
                {
                    // Set the projectile to shoot towords center mass and not legs
                    float playerYOffset = EnmyAi.GetAttackingPlayer().GetComponent<CharacterController>().center.y;
                    Vector3 playerPosition = EnmyAi.GetAttackingPlayer().transform.position + new Vector3(0f, playerYOffset, 0f);
                    // Shooting direction
                    Vector3 shootDirection = playerPosition - transform.position;
                    // Instantiate projectile
                    GameObject currentProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    // Rotate projectile to shoot direction
                    currentProjectile.transform.forward = shootDirection;
                    // Add forces to projectile
                    currentProjectile.GetComponent<Rigidbody>().AddForce(shootDirection.normalized * projectileSpeed, ForceMode.Impulse);
                    // Add damage to the projectile
                    int damage = (int)(this.EnmyStts.getAttackDamage() * damageModifier);
                    currentProjectile.GetComponent<Projectile.HandleHitsAndDamage>().setDamage(damage);
                }
            }
        }
    }
}

