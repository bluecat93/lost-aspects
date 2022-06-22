using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Object
{
    public class InteractableObject : NetworkBehaviour
    {
        [SerializeField] int ID;
        [SyncVar] private bool IsPickedUp = false;
        void OnTriggerStay(Collider other)
        {
            if (other.tag == Finals.PLAYER && Input.GetButtonDown(Finals.USE) && !IsPickedUp)
            {
                if (other.GetComponent<Player.Stats>().hasAuthority)
                {
                    bool isItemAdded = other.GetComponent<Inventory.InventoryManager>().AddItem(ID);

                    // if item is added then we remove the item object from the game.
                    if (isItemAdded)
                    {
                        if (isServer)
                        {
                            IsPickedUp = true;
                            RPCDestroyObjectForEveryone();
                        }
                        else if (isClient)
                        {
                            CMDDestroyObjectForEveryone();
                        }
                    }
                    else
                    {
                        // Item was not added to inventory
                        // TODO is this else needed?
                    }
                }
            }
        }

        [Command]
        private void CMDDestroyObjectForEveryone()
        {
            IsPickedUp = true;
            RPCDestroyObjectForEveryone();
        }
        [ClientRpc]
        private void RPCDestroyObjectForEveryone()
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
