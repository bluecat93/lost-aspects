using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public class ItemList
    {
        // instead of using ID, we will use the item's index as ID.
        // [Tooltip("Do not use negative numbers or zero as IDs")]
        // [SerializeField] private int ID;
        [SerializeField] private string name;
        [Tooltip("list of types that this item can restore and its amount. If this item is not conumable then leave it empty.")]
        [SerializeField] private List<ConsumableStats> RestorationList;

        // [SerializeField] private bool isEquipable;
        [SerializeField] private int maxStack = Finals.DEFAULT_MAX_STACK;
        [Tooltip("The picture that will show in canvas in a sprite format")]
        [SerializeField] private Sprite sprite;

        // enum health hunger
        // list of enum above what can the consumable rise


        public bool IsConsumable()
        {
            return RestorationList.Count > 0;
        }
        // public bool IsEquipable()
        // {
        //     return isEquipable;
        // }
        public int GetMaxStack()
        {
            return maxStack;
        }
        public Sprite GetSprite()
        {
            return sprite;
        }
        public List<ConsumableStats> GetRestorationList()
        {
            return RestorationList;
        }
    }
}
