using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Object
{
    public class InteractableObject : NetworkBehaviour
    {
        [SerializeField] int ID;
        private bool IsPickedUp = false;
        void OnTriggerStay(Collider other)
        {
            if (other.tag == Finals.PLAYER && Input.GetKeyDown(Finals.USE) && !IsPickedUp)
            {
                if (other.GetComponent<Player.Stats>().hasAuthority)
                {
                    IsPickedUp = true;
                    bool isItemAdded = other.GetComponent<Inventory.InventoryManager>().AddItem(ID);
                    if (isItemAdded)
                    {
                        if (isServer)
                        {
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
            RPCDestroyObjectForEveryone();
        }
        [ClientRpc]
        private void RPCDestroyObjectForEveryone()
        {
            DestroyObjectForEveryone();
        }
        private void DestroyObjectForEveryone()
        {
            IsPickedUp = true;
            // TODO add piuckup animations here
            Destroy(this, Finals.ITEM_PICKUP_TIME);
        }
    }
}
