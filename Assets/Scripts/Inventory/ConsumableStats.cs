using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable]
    public enum Restoration
    {
        Health,
        Hunger
    }
    [Serializable]
    public class ConsumableStats
    {
        public ConsumableStats()
        {
            restorationType = Restoration.Health;
            amount = 0;
        }

        public ConsumableStats(Restoration restorationType, int amount)
        {
            this.restorationType = restorationType;
            this.amount = amount;
        }

        public Restoration restorationType = Restoration.Health;
        public int amount = 0;
    }
}
