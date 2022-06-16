using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        private class ItemInsideInventory
        {
            public int ID = -1;
            public int count = 1;
        }

        #region variables
        [SerializeField] private int InventorySize;
        public InventoryIndex inventoryIndex;

        private List<ItemInsideInventory> Items;

        #endregion

        // add a single item. returns true if item was added successfuly
        public bool AddItem(int ID)
        {
            // searching for the item to add to an existing stack
            foreach (ItemInsideInventory item in Items)
            {
                if (ID == item.ID)
                {
                    if (item.count < inventoryIndex.GetItemByID(item.ID).GetMaxStack())
                    {
                        item.count++;
                        return true;
                    }
                }
            }

            // no stack found so trying to add one if we have room in inventory
            if (Items.Count < InventorySize)
            {
                // create new stack
                ItemInsideInventory item = new ItemInsideInventory();
                item.count = 1;
                item.ID = ID;

                // add new stack to inventory
                Items.Add(item);

                return true;
            }

            // inventory is full
            else
            {
                Debug.Log("Inventory is full");
                return false;
            }
        }

        // remove a single item. return true if item was removed successfuly
        public bool RemoveItem(int ID)
        {
            // searching for the item to remove from an existing stack
            foreach (ItemInsideInventory item in Items)
            {
                if (ID == item.ID)
                {
                    if (item.count < inventoryIndex.GetItemByID(item.ID).GetMaxStack())
                    {
                        item.count--;

                        // if stack is empty then we need to remove it from inventory so it wont take up space
                        if (item.count == 0)
                        {
                            Items.Remove(item);
                        }

                        return true;
                    }
                }
            }
            return false;
        }

    }

}
