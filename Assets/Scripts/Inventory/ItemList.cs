using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public class ItemList
    {
        [Tooltip("Do not use negative numbers or zero as IDs")]
        [SerializeField] private int ID;
        [SerializeField] private bool isConsumable;
        [SerializeField] private bool isEquipable;
        [SerializeField] private int maxStack = Finals.DEFAULT_MAX_STACK;
        [Tooltip("The picture that will show in canvas in a sprite format")]
        [SerializeField] private Sprite sprite;

        public int GetID()
        {
            return ID;
        }
        public bool IsConsumable()
        {
            return isConsumable;
        }
        public bool IsEquipable()
        {
            return isEquipable;
        }
        public int GetMaxStack()
        {
            return maxStack;
        }
        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
