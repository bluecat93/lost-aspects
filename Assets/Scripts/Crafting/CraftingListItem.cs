using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using Inventory;

namespace Crafting
{
    public class CraftingListItem : MonoBehaviour
    {
        private Recipe MyRecipe;
        public List<GameObject> IngridientsObjects;
        public GameObject CraftedItem;
        private InventoryManager _inventoryManager = null;
        private InventoryManager InvntryMngr
        {
            get
            {
                if (_inventoryManager == null)
                    _inventoryManager = GetComponentInParent<InventoryManager>();
                return _inventoryManager;
            }
        }
        public void SetData(Recipe recipe)
        {
            MyRecipe = recipe;
            for (int i = 0; i < IngridientsObjects.Count; i++)
            {
                if (recipe.Ingridients.Count > i)
                {
                    SetSingleItem(IngridientsObjects[i], recipe.Ingridients[i]);
                }
                // rest of ingridient will be shown as empty
                else
                {
                    SetSingleItem(IngridientsObjects[i], new InventoryManager.ItemInsideInventory());
                }
            }
            SetSingleItem(CraftedItem, recipe.CraftedItem);
        }
        private void SetSingleItem(GameObject obj, InventoryManager.ItemInsideInventory item)
        {
            ItemList itemList = InvntryMngr.InventoryIndexList.GetItemByID(item.ID);
            obj.GetComponentInChildren<Image>().sprite = itemList.GetSprite();

            if (item.ID != 0)
            {
                obj.GetComponentInChildren<Text>().text = "" + item.count;
            }
        }

        // clicked on the craft button.
        public void Craft()
        {
            if (InvntryMngr.IsCraftable(MyRecipe.Ingridients))
            {
                // remove all ingridients from inventory
                foreach (InventoryManager.ItemInsideInventory ingridient in MyRecipe.Ingridients)
                {
                    for (int i = 0; i < ingridient.count; i++)
                    {
                        InvntryMngr.RemoveItemByID(ingridient.ID);
                    }
                }
                // add all crafted items to inventory
                for (int i = 0; i < MyRecipe.CraftedItem.count; i++)
                {
                    if (!InvntryMngr.AddItem(MyRecipe.CraftedItem.ID))
                    {
                        // no inventory space
                        Debug.Log("Can't craft item, missing inventory space");

                        // remove all items created
                        // i = 0 meaning there are no items crafted, it will not enter here in that case.
                        for (int j = i - 1; j >= 0; j--)
                        {
                            InvntryMngr.RemoveItemByID(MyRecipe.CraftedItem.ID);
                        }

                        // add all ingridients back to inventory.
                        foreach (InventoryManager.ItemInsideInventory ingridient in MyRecipe.Ingridients)
                        {
                            for (int j = 0; j < ingridient.count; j++)
                            {
                                InvntryMngr.AddItem(ingridient.ID);
                            }
                        }

                    }
                }
            }
            else
            {
                Debug.Log("Can't craft item, missing ingridients");

                // TODO add list of missing ingridients?
            }

        }
    }
}