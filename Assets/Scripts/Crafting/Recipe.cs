using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory;


namespace Crafting
{
    [Serializable]
    public class Recipe
    {
        public List<InventoryManager.ItemInsideInventory> Ingridients;
        public InventoryManager.ItemInsideInventory CraftedItem;
    }
}