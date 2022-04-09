using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectile
{
    public class HandleHitsAndDamage : MonoBehaviour
    {
        [Tooltip("time in seconds untill the projectile will disappear")]
        [SerializeField] private float deathTimer;
        // Start is called before the first frame update
        [Tooltip("The Tags of who this projectile can hit")]
        [SerializeField] List<string> taglist;
        private int damage = 0;
        void Start()
        {
            Destroy(gameObject, deathTimer);
        }

        private void OnTriggerStay(Collider other)
        {
            foreach (string tag in taglist)
            {
                if (other.gameObject.tag == tag)
                {
                    if (tag == "player")
                    {
                        other.GetComponent<Player.Stats>().TakeDamage(damage);
                    }
                    else if (tag == "enemy")
                    {
                        other.GetComponent<Enemy.Stats>().TakeDamage(damage);
                    }
                    Destroy(gameObject, deathTimer);
                }
            }
        }
        public void setDamage(int damage)
        {
            this.damage = damage;
        }
    }
}
