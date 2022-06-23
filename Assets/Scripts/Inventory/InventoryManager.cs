using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        public class ItemInsideInventory
        {
            // constructor, ID = 0 meaning its an empty item.
            public ItemInsideInventory()
            {
                ID = 0;
                count = 1;
            }
            public ItemInsideInventory(int ID, int count)
            {
                this.ID = ID;
                this.count = count;
            }
            public int ID { get; }
            public int count { get; set; }

            public override string ToString()
            {
                return "ID = " + ID + "\tcount = " + count;
            }
        }

        #region variables
        [SerializeField] private int InventorySize;
        private InventoryIndex _inventoryIndex;

        [HideInInspector]
        public InventoryIndex InventoryIndexList
        {
            get
            {
                if (this._inventoryIndex == null)
                    this._inventoryIndex = GameObject.Find(Finals.ITEM_INDEX).GetComponent<InventoryIndex>();

                return this._inventoryIndex;
            }
        }

        private Player.Stats _playerStats;

        [HideInInspector]
        private Player.Stats PlayerStats
        {
            get
            {
                if (this._playerStats == null)
                    this._playerStats = GetComponent<Player.Stats>();

                return this._playerStats;
            }
        }

        private List<ItemInsideInventory> Items = new List<ItemInsideInventory>();

        void Start()
        {
            for (int i = 0; i < InventorySize; i++)
            {
                Items.Add(new ItemInsideInventory());
            }
        }

        #endregion

        // add a single item. returns true if item was added successfuly
        public bool AddItem(int ID)
        {
            bool hasEmptySlot = false;
            int emptySlotIndex = -1;
            // searching for the item to add to an existing stack
            for (int i = 0; i < InventorySize; i++)
            {
                ItemInsideInventory item = Items[i];

                if (ID == item.ID)
                {
                    if (item.count < InventoryIndexList.GetItemByID(item.ID).GetMaxStack())
                    {
                        item.count++;
                        return true;
                    }
                }

                // get first empty slot
                if (item.ID == 0 && !hasEmptySlot)
                {
                    hasEmptySlot = true;
                    emptySlotIndex = i;
                }
            }
            // no stack found so trying to add one if we have room in inventory
            if (hasEmptySlot)
            {
                // create new stack
                ItemInsideInventory item = new ItemInsideInventory(ID, 1);

                // add new stack to inventory
                Items[emptySlotIndex] = item;

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
        public bool RemoveItemByID(int ID)
        {
            // searching for the item to remove from an existing stack
            for (int i = 0; i < InventorySize; i++)
            {
                ItemInsideInventory item = Items[i];

                if (ID == item.ID)
                {
                    if (item.count < InventoryIndexList.GetItemByID(item.ID).GetMaxStack())
                    {
                        item.count--;

                        // if stack is empty then we need to remove it from inventory so it wont take up space
                        if (item.count == 0)
                        {
                            Items[i] = new ItemInsideInventory();
                        }

                        return true;
                    }
                }
            }
            return false;
        }
        public bool RemoveItemByIndex(int index)
        {
            ItemInsideInventory item = Items[index];

            if (index >= 0 && index < InventorySize)
            {
                item.count--;

                // if stack is empty then we need to remove it from inventory so it wont take up space
                if (item.count == 0)
                {
                    Items[index] = new ItemInsideInventory();
                }

                return true;
            }
            return false;
        }


        public bool RemoveItemStack(int index)
        {
            // searching for the item to remove from an existing stack
            if (index >= 0 && index < InventorySize)
            {
                Items[index] = new ItemInsideInventory();
                return true;
            }
            return false;
        }

        public void SwapItems(ItemInsideInventory item1, ItemInsideInventory item2)
        {
            int index1 = Items.IndexOf(item1);
            int index2 = Items.IndexOf(item2);
            if (index1 >= 0 && index2 >= 0)
            {
                Items[index1] = item2;
                Items[index2] = item1;
            }
        }

        public void Consume(int index)
        {

            ItemList item = InventoryIndexList.GetItemByID(Items[index].ID);
            foreach (ConsumableStats stat in item.GetRestorationList())
            {
                switch (stat.restorationType)
                {
                    case Restoration.Health:
                        PlayerStats.Heal(stat.amount);
                        break;
                    case Restoration.Hunger:
                        PlayerStats.Eat(stat.amount);
                        break;
                }
            }
            RemoveItemByIndex(index);
        }

        public int GetInventorySize()
        {
            return InventorySize;
        }
        public List<ItemInsideInventory> GetItems()
        {
            return Items;
        }
    }

}
