using ModuleBatteries.Behaviours;
using ModuleBatteries.Items;
using UnityEngine;
using UWE;
using System.Collections;

namespace ModuleBatteries.Components
{
    public class NuclearBatteryComponent : CustomBatteryBase
    {
        protected override bool IsValidFuel(Pickupable item) =>
            item != null && item.GetTechType() == ReactorRodItems.MiniRod;

        protected override void CreateContainer()
        {
            container = new ItemsContainer(
                InventoryDimensions.x,
                InventoryDimensions.y,
                transform,
                string.Empty,
                null // optionally define an FMODAsset error sound
            );

            container.isAllowedToAdd = (pickupable, verbose) =>
            {
                var techType = pickupable.GetTechType();

                if (techType != ReactorRodItems.MiniRod && techType != ReactorRodItems.DepletedMiniRod)
                {
                    if (verbose)
                        ErrorMessage.AddMessage("Only Mini Reactor Rods are allowed in Nuclear Battery.");
                    return false;
                }

                return true;
            };

            container.onAddItem += OnItemAdded;
            container.onRemoveItem += OnItemRemoved;
        }


        protected override int GetCapacity()
        {
            int capacity = 0;
            foreach (var item in container)
            {
                if (item?.item != null && item.item.GetTechType() == ReactorRodItems.MiniRod)
                {
                    capacity += 1000;
                }
            }
            return Mathf.Max(1, capacity); // Never return 0
        }

        protected override float GetChargeRate()
        {
            int totalRate = 0;
            foreach (var item in container)
            {
                if (item?.item != null && item.item.GetTechType() == ReactorRodItems.MiniRod)
                {
                    totalRate += 10; // Each rod provides 10 charge per tick
                }
            }

            return totalRate; // Return 0 if no rods are present
        }


        protected override bool ShouldConsumeFuel() => true;

        protected override bool IsFuelLocked() => true;

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

            // Clamp existing charge to new capacity
            battery.charge = Mathf.Clamp(battery.charge, 0f, newCapacity);
        }

        protected override void TryConsumeFuel()
        {
            if (battery.capacity <= 0 || float.IsNaN(battery.capacity))
            {
                Debug.LogWarning("[NuclearBattery] Skipping fuel tick due to invalid capacity.");
                return;
            }

            foreach (var item in container)
            {
                if (item?.item == null || item.item.GetTechType() != ReactorRodItems.MiniRod)
                    continue;

                var tag = item.item.GetComponent<FuelTag>();
                if (tag == null)
                {
                    tag = item.item.gameObject.AddComponent<FuelTag>();
                    tag.remainingEnergy = 1000f;
                }

                float perTick = 10f;
                float toAdd = Mathf.Min(perTick, tag.remainingEnergy);
                battery.charge = Mathf.Min(battery.capacity, battery.charge + toAdd);
                tag.remainingEnergy -= toAdd;

                if (tag.remainingEnergy <= 0f)
                {
                    container.RemoveItem(item.item, true);
                    CoroutineHost.StartCoroutine(SpawnDepletedRod(container));
                    SyncBatteryStats();
                }

                break; // Consume one rod at a time
            }
        }

        private static IEnumerator SpawnDepletedRod(ItemsContainer container)
        {
            var result = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(ReactorRodItems.DepletedMiniRod, result);

            GameObject prefab = result.Get();
            if (prefab == null)
            {
                Debug.LogError("[NuclearBattery] Failed to spawn DepletedMiniRod.");
                yield break;
            }

            Pickupable pickup = prefab.GetComponent<Pickupable>();
            if (pickup != null)
            {
                container.UnsafeAdd(pickup.inventoryItem);
            }
            else
            {
                Debug.LogError("[NuclearBattery] Spawned rod has no Pickupable.");
            }
        }
    }
}