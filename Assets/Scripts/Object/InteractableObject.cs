using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Object
{
    public class InteractableObject : NetworkBehaviour
    {
        [SerializeField] int ID;
        [SyncVar][HideInInspector] public bool IsPickedup = false;
        private bool IsClientPickedup = false;
        private Collider LastPlayerCollider;
        void OnTriggerStay(Collider other)
        {
            if (other.tag == Finals.PLAYER && Input.GetAxis(Finals.USE) != 0 && !IsPickedup)
            {
                if (other.GetComponent<Player.Stats>().hasAuthority)
                {
                    LastPlayerCollider = other;
                    if (isServer)
                    {
                        if (!IsPickedup)
                        {
                            IsPickedup = true;
                            HandleItemPickup(other);
                        }
                    }
                    else if (isClient)
                    {
                        other.GetComponent<Player.Abilities>().CmdItemPickup(this);
                    }
                }
            }
        }



        // server sends the okay to pick up item for a specific client
        [TargetRpc]
        public void TargetItemPickup(NetworkIdentity identity)
        {
            Debug.Log("im in targetRPC");
            HandleItemPickup(LastPlayerCollider);
        }

        // item pickup after checking if item was picked up.
        private void HandleItemPickup(Collider other)
        {
            bool isItemAdded = other.GetComponent<Inventory.InventoryManager>().AddItem(ID);

            // if item is added then we remove the item object from the game.
            if (isItemAdded)
            {
                if (isServer)
                {
                    RPCDestroyObjectForEveryone();
                }
                else if (isClient && !IsClientPickedup)
                {
                    IsClientPickedup = true;
                    IsPickedup = true;
                    // need to give authority over the item for the client (can only be done by the server). 
                    // only then do from server as normal (see above).
                    other.GetComponent<Player.Abilities>().GiveAuthorityForItemPickup(this);
                }
            }
            else
            {
                // item was not picked up (maybe inventory was full or something else happened).
                // reset item pickup for all clients and host
                if (isServer)
                {
                    IsPickedup = false;
                }
                else if (isClient)
                {
                    IsClientPickedup = false;
                    CmdCancelItemPickup();
                }
            }
        }

        [Command]
        private void CmdCancelItemPickup()
        {
            IsPickedup = false;
        }

        [ClientRpc]
        public void RPCDestroyObjectForEveryone()
        {
            DestroyObjectForEveryone();
        }
        private void DestroyObjectForEveryone()
        {
            // TODO add piuckup animations here
            Destroy(this.gameObject, Finals.ITEM_PICKUP_TIME);
        }
    }
}
