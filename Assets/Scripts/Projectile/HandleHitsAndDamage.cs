using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectile
{
    public class HandleHitsAndDamage : MonoBehaviour
    {
        // The tag list types that can be interactable.
        enum Type
        {
            Player,
            Enemy,
            Equipable
        }
        [Tooltip("Time in seconds untill the projectile will disappear")]
        [SerializeField] private float deathTimer;
        [Tooltip("The type of who this projectile can hit")]
        [SerializeField] List<Type> taglist;

        private int damage = 0;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, deathTimer);
        }

        // Run on all tags that can be hit by the projectile. If found one then do the damage. In any case, destroy the object after
        private void OnTriggerStay(Collider other)
        {
            // Debug.Log("collided with: " + other + "\tand its tag is: " + other.tag);
            foreach (Type type in taglist)
            {
                // Convert the type enum to tag.
                string tag = type.ToString();
                if (other.gameObject.tag == tag)
                {
                    // Got a hit with something the projectile can hit.
                    if (tag == "Player")
                    {
                        other.GetComponent<Player.Stats>().TakeDamage(damage);
                    }
                    else if (tag == "Enemy")
                    {
                        other.GetComponent<Enemy.Stats>().TakeDamage(damage);
                    }
                    else if (tag == "Equipable")
                    {
                        other.GetComponentInParent<Player.Stats>().TakeDamage(damage);
                    }
                }
            }
            // After got a hit with anything do this: 

            // Destroy game object now.
            Destroy(gameObject, 0);
        }
        public void setDamage(int damage)
        {
            this.damage = damage;
        }
    }
}
