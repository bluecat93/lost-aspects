using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{
    public class InventoryIndex : MonoBehaviour
    {
        [Tooltip("The list of all items that can be inside the players inventory.")]
        [SerializeField] private List<ItemList> itemList;

        public List<ItemList> GetItemList()
        {
            return itemList;
        }

        public ItemList GetItemByID(int ID)
        {
            foreach (ItemList item in itemList)
            {
                if (item.GetID() == ID)
                    return item;
            }
            return null;
        }
    }
}
