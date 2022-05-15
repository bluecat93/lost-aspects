using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object;
using UnityEngine.Events;
using Animation;
using Mirror;

namespace Player
{
    public class Abilities : NetworkBehaviour
    {
        [Header("Animations")]
        [Tooltip("The default animator controller.")]
        [SerializeField] private RuntimeAnimatorController defaultAnimatorController;

        private Animator _animator;

        private Animator Anmtor
        {
            get
            {
                if (this._animator == null)
                    this._animator = GetComponentInChildren<Animator>();

                return this._animator;
            }
        }

        ThirdPersonMovement _ThirdPersonMovement;

        private ThirdPersonMovement ThrdPrsnMvmnt
        {
            get
            {
                if (this._ThirdPersonMovement == null)
                    this._ThirdPersonMovement = GetComponent<ThirdPersonMovement>();

                return this._ThirdPersonMovement;
            }
        }

        private EventsHandler _eventHandler;

        private bool isEquiped = false;

        // Update is called once per frame
        void Update()
        {
            // only the active client can enter this if because of hasAuthority
            if (ThrdPrsnMvmnt.IsExist && hasAuthority)
            {
                if (Input.GetButtonDown("Equip"))
                {
                    CmdEquip();
                }
            }

        }

        //Equip or unequip a weapon. NOTE: this will only work with one weapon type. if we will have more than we have to insantiate them in the right positions by getting the offsets and the like.
        #region Weapon Equpiment
        public void EquipWeapon()
        {
            WeaponAttack meleeWeapon = GetComponentInChildren<WeaponAttack>(true);
            meleeWeapon.gameObject.SetActive(true);
            meleeWeapon.Equip();
        }
        public void unequipWeapon()
        {
            WeaponAttack meleeWeapon = GetComponentInChildren<WeaponAttack>(true);
            meleeWeapon.gameObject.SetActive(false);
            Functions.SetAnimatorController(Anmtor, this.defaultAnimatorController);

        }

        // This function is called from client who clicked on equip button and is runed by server.
        [Command]
        private void CmdEquip()
        {
            RpcEquip();
        }

        // This function is called from server (and is used in the Cmd function above) and is runed by all clients including the server.
        [ClientRpc]
        private void RpcEquip()
        {
            if (isEquiped)
            {
                unequipWeapon();
            }
            else
            {
                EquipWeapon();
            }
            isEquiped = !isEquiped;
        }


        #endregion

    }

}