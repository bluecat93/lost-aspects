using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Object
{
    public class InteractableObject : NetworkBehaviour
    {
        [SerializeField] int ID;
        [SyncVar][HideInInspector] public bool IsPickedUp = false;
        private bool IsClientPickedUp = false;
        void OnTriggerStay(Collider other)
        {
            if (other.tag == Finals.PLAYER && Input.GetAxis(Finals.USE) != 0 && !IsPickedUp)
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
                        else if (isClient && !IsClientPickedUp)
                        {
                            IsClientPickedUp = true;
                            IsPickedUp = true;
                            // need to give authority over the item for the client (can only be done by the server). 
                            // only then do from server as normal (see above).
                            other.GetComponent<Player.Abilities>().GiveAuthorityForItemPickup(this);
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
