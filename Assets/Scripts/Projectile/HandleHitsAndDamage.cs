using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Projectile
{
    public class HandleHitsAndDamage : NetworkBehaviour
    {
        // The tag list types that can be interactable.
        enum Type
        {
            Player,
            Enemy,
            Equipable,
            Terrain
        }

        [Tooltip("Time in seconds untill the projectile will disappear")]
        [SerializeField] private float deathTimer;
        [Tooltip("The type of who this projectile can deal damage to")]
        [SerializeField] List<Type> hitList;
        [Tooltip("The type of who this projectile will be destoyed by when hit.")]
        [SerializeField] List<Type> destructionOnHitList;

        private int damage = 0;
        //used when something special happens that makes the projectile not destroy itself.
        private bool evadeDestruction = false;
        private bool attackOnlyOnce = false;
        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, deathTimer);
        }

        // Run on all tags that can be hit by the projectile. If found one then do the damage. In any case, destroy the object after
        private void OnTriggerStay(Collider other)
        {

            // Debug.Log("collided with: " + other + "\tand its tag is: " + other.tag);
            foreach (Type type in hitList)
            {
                // Convert the type enum to tag.
                string tag = type.ToString();
                if (other.gameObject.tag.Equals(tag))
                {
                    if (!attackOnlyOnce)
                    {
                        attackOnlyOnce = true;
                        // Got a hit with something the projectile can hit.
                        if (tag.Equals("Player"))
                        {
                            other.GetComponent<Player.Stats>().TakeDamage(damage);
                        }
                        else if (tag.Equals("Enemy"))
                        {
                            other.GetComponent<Enemy.Stats>().TakeDamage(damage);
                        }
                        else if (tag.Equals("Equipable"))
                        {
                            other.GetComponentInParent<Player.Stats>().TakeDamage(damage);
                        }
                    }
                }
            }
            // After got a hit with anything do this: 
            foreach (Type type in destructionOnHitList)
            {
                if (other.tag.Equals(type.ToString()))
                {
                    if (type == Type.Player)
                    {
                        evadeDestruction = other.GetComponent<Player.ThirdPersonMovement>().IsRolling;
                    }
                    else if (type == Type.Equipable)
                    {
                        evadeDestruction = other.GetComponentInParent<Player.ThirdPersonMovement>().IsRolling;
                    }
                    if (!evadeDestruction)
                    {
                        // Destroy game object now.
                        Destroy(gameObject, 0);
                    }
                }
            }

        }

        public void setDamage(int damage)
        {
            this.damage = damage;
        }
    }
}
