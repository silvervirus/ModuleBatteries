using ModuleBatteries.Behaviours;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModuleBatteries.Components
{
    public class ModuleBatteryComponent : CustomBatteryBase
    {
        private static readonly List<BatteryUpgradeEffect> Upgrades = new()
        {
            new BatteryUpgradeEffect(TechType.Copper, capacity: 100, charge: 10f),
            new BatteryUpgradeEffect(TechType.Silver, charge: 5f),
            new BatteryUpgradeEffect(TechType.PrecursorIonCrystal, capacity: 500),
            new BatteryUpgradeEffect(TechType.Gold, toolBoost: true),
            new BatteryUpgradeEffect(TechType.Lithium),
            new BatteryUpgradeEffect(TechType.AluminumOxide)
        };

        protected override int InventorySize => 4;
        protected override Vector2int InventoryDimensions => new(2, 2);
        protected override bool ShouldConsumeFuel() => false;
        protected override void TryConsumeFuel() { }
        protected virtual int GetBaseCapacity() => 300;

        protected override int GetCapacity()
        {
            int capacity = GetBaseCapacity();
            foreach (var item in container)
            {
                var match = Upgrades.Find(u => u.TechType == item.item.GetTechType());
                if (match != null)
                    capacity += match.CapacityBonus;
            }
            return capacity;
        }

        protected override float GetChargeRate()
        {
            float rate = 10f;
            foreach (var item in container)
            {
                var match = Upgrades.Find(u => u.TechType == item.item.GetTechType());
                if (match != null)
                    rate += match.ChargeRateBonus;
            }
            return rate;
        }

        protected override bool IsValidFuel(Pickupable item)
        {
            if (item == null) return false;
            return Upgrades.Any(u => u.TechType == item.GetTechType());
        }

        public bool HasToolBoost()
        {
            foreach (var item in container)
            {
                var match = Upgrades.Find(u => u.TechType == item.item.GetTechType());
                if (match != null && match.GrantsToolBoost)
                    return true;
            }
            return false;
        }

        protected override void OnItemAdded(InventoryItem item)
        {
            base.OnItemAdded(item);
            SyncBatteryStats();
        }

        protected override void OnItemRemoved(InventoryItem item)
        {
            base.OnItemRemoved(item);
            SyncBatteryStats();
        }

        private void SyncBatteryStats()
        {
            int newCapacity = GetCapacity();
            battery._capacity = newCapacity;
            battery.charge = Mathf.Min(battery.charge, newCapacity);
        }

        protected override void OnInitialized()
        {
           
            SyncBatteryStats();
        }
    }
}
