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

            public override string ToString()
            {
                return "ID = " + ID + "\tcount = " + count;
            }
        }

        #region variables
        [SerializeField] private int InventorySize;
        private InventoryIndex _inventoryIndex;

        private InventoryIndex InventoryIndexList
        {
            get
            {
                if (this._inventoryIndex == null)
                    this._inventoryIndex = GameObject.Find(Finals.ITEM_INDEX).GetComponent<InventoryIndex>();

                return this._inventoryIndex;
            }
        }

        private List<ItemInsideInventory> Items = new List<ItemInsideInventory>();

        #endregion

        // add a single item. returns true if item was added successfuly
        public bool AddItem(int ID)
        {
            // searching for the item to add to an existing stack
            foreach (ItemInsideInventory item in Items)
            {
                if (ID == item.ID)
                {
                    if (item.count < InventoryIndexList.GetItemByID(item.ID).GetMaxStack())
                    {
                        item.count++;
                        //TODO remove this log
                        Debug.Log(string.Join(", ", Items));
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
                //TODO remove this log
                Debug.Log(string.Join(", ", Items));
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
                    if (item.count < InventoryIndexList.GetItemByID(item.ID).GetMaxStack())
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

        public void SwapItems(int index1, int index2)
        {
            if (index1 > 0 && index2 > 0 && index1 < InventorySize && index2 < InventorySize)
            {
                ItemInsideInventory temp = Items[index1];
                Items[index1] = Items[index2];
                Items[index2] = temp;
            }
        }
    }

}
