using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animation;
using Mirror;

namespace Object
{
    public class WeaponAttack : MonoBehaviour
    {

        [Header("Animations")]
        [Tooltip("The animator controller that this weapon uses.")]
        [SerializeField] private RuntimeAnimatorController animatorController;

        [Header("Damage")]
        [Tooltip("The base weapon damage is added to every attack in the weapon.")]
        [SerializeField] private int weaponBaseAttackDamage;
        [Tooltip("The damage modifiers of each weapon attack. If you have X attacks this array has to be in the size of X.")]
        [SerializeField] private float[] attackDamageModifier;


        private bool isAttacking;
        private int currentAttackIndex;

        private EventsHandler _eventHandler;

        private EventsHandler EvntHndlr
        {
            get
            {
                if (this._eventHandler == null)
                    this._eventHandler = GetComponentInParent<EventsHandler>();
                return this._eventHandler;
            }
        }

        private Animator _animator;

        private Animator Anmtor
        {
            get
            {
                if (this._animator == null)
                    this._animator = GetComponentInParent<Animator>();

                return this._animator;
            }
        }

        private NetworkAnimator _NetworkAnimator;

        private NetworkAnimator NetworkAnim
        {
            get
            {
                if (this._NetworkAnimator == null)
                    this._NetworkAnimator = GetComponentInParent<NetworkAnimator>();

                return this._NetworkAnimator;
            }
        }


        void Awake()
        {
            this.EvntHndlr.OnAttackEventTrigger.AddListener(SetAttackParamaters);
        }

        void OnEnable()
        {
            isAttacking = false;
            currentAttackIndex = 0;
        }

        // Update is called once per frame
        void Update()
        {
            // float attacking = Input.GetAxisRaw("Swing");
            if (Input.GetButtonDown(Finals.ATTACK) && !this.isAttacking)
            {
                this.Anmtor.SetInteger(Finals.ATTACK_NUMBER_PLAYER, currentAttackIndex + 1);
                // setTrigger now needs to be called from network animator and not animator.
                this.NetworkAnim.SetTrigger(Finals.ATTACK_PLAYER);
                // this.Anmtor.SetTrigger("Attack");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == Finals.ENEMY && this.isAttacking)
            {
                //enemy was hit
                Transform enemy = other.gameObject.transform;
                Enemy.Stats enemyStats = enemy.GetComponent<Enemy.Stats>();
                int finalDamage = (int)(this.weaponBaseAttackDamage * this.attackDamageModifier[this.currentAttackIndex]);
                enemyStats.TakeDamage(finalDamage);

                // Debug.Log("enemy hit with attack number: " + GetComponentInParent<Player.Abilities>().attackNumber);
            }
        }

        public void SetAttackParamaters(bool isAttacking)
        {
            this.isAttacking = isAttacking;
            if (!isAttacking)
            {
                currentAttackIndex = currentAttackIndex >= attackDamageModifier.Length - 1 ? 0 : currentAttackIndex + 1;
            }
            // Debug.Log("attack started? " + isAttacking + "\tattack number right after the change: " + currentAttackIndex);

        }

        public void Equip()
        {
            Functions.SetAnimatorController(GetComponentInParent<Animator>(), this.animatorController);
        }
    }

}