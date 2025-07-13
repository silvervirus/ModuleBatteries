using System.Collections;
using UnityEngine;
using ModuleBatteries.Items; // For ReactorRodItems
using UWE;

namespace ModuleBatteries.Behaviours
{
    public abstract class CustomBatteryBase : MonoBehaviour
    {
        protected Battery battery;
        protected ItemsContainer container;

        protected virtual int InventorySize => 1;
        protected virtual Vector2int InventoryDimensions => new(1, 1);

        private void Start()
        {
            battery = GetComponent<Battery>() ?? gameObject.EnsureComponent<Battery>();
            CreateContainer();
            OnInitialized();
            StartCoroutine(EnergyTick());
            container = GetComponentInChildren<StorageContainer>()?.container;
            if (container == null)
                Debug.LogError("[NuclearBattery] Container not found!");
            else
                Debug.Log("[NuclearBattery] Container found and initialized.");

            CoroutineHost.StartCoroutine(FuelLoop());
        }
      
        private IEnumerator FuelLoop()
        {
            while (true)
            {
                TryConsumeFuel();
                yield return new WaitForSeconds(1f);
            }
        }


        protected virtual void CreateContainer()
        {
            container = new ItemsContainer(
                InventoryDimensions.x,
                InventoryDimensions.y,
                gameObject.transform,
                null,
                null);

            container.onAddItem += OnItemAdded;
            container.onRemoveItem += OnItemRemoved;
        }

        protected virtual IEnumerator EnergyTick()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (IsActive() && battery.charge < battery.capacity)
                {
                    battery.charge = Mathf.Min(battery.capacity, battery.charge + GetChargeRate());

                    if (ShouldConsumeFuel())
                        TryConsumeFuel();
                }
            }
        }

        protected virtual bool IsActive()
        {
            foreach (var item in container)
            {
                if (IsValidFuel(item.item))
                    return true;
            }
            return false;
        }

        protected virtual void TryConsumeFuel()
        {
            foreach (var item in container)
            {
                if (IsValidFuel(item.item))
                {
                    var techType = item.item.GetTechType();

                    if (techType == ReactorRodItems.MiniRod)
                    {
                        container.RemoveItem(item.item, true);
                        UWE.CoroutineHost.StartCoroutine(SpawnDepletedRod(container));
                    }
                    else
                    {
                        container.RemoveItem(item.item, true);
                    }

                    break;
                }
            }
        }
        protected virtual void OnInitialized() { }
        private static IEnumerator SpawnDepletedRod(ItemsContainer container)
        {
            var result = new TaskResult<GameObject>();
            yield return CraftData.InstantiateFromPrefabAsync(ReactorRodItems.DepletedMiniRod, result);

            GameObject prefab = result.Get();
            if (prefab == null)
            {
                Debug.LogError("[NuclearBattery] Failed to instantiate DepletedMiniRod prefab.");
                yield break;
            }

            var pickup = prefab.GetComponent<Pickupable>();
            if (pickup != null)
            {
                container.UnsafeAdd(pickup.inventoryItem);
            }
            else
            {
                Debug.LogError("[NuclearBattery] Instantiated prefab has no Pickupable.");
            }
        }

        protected virtual void OnItemAdded(InventoryItem item)
        {
            if (!IsValidFuel(item.item))
            {
                container.RemoveItem(item.item, true);
                ErrorMessage.AddMessage("Invalid item for this battery.");
                return;
            }

            if (!ShouldConsumeFuel())
                return; // No fuel logic needed (e.g. upgrade batteries)

            if (IsFuelLocked())
            {
                var pickup = item.item.GetComponent<Pickupable>();
                if (pickup != null)
                {
                    pickup.isPickupable = false;
                }
            }

            if (item.item.GetComponent<FuelTag>() == null)
            {
                item.item.gameObject.AddComponent<FuelTag>().remainingEnergy = 1000f;
            }
        }

        protected virtual void OnItemRemoved(InventoryItem item)
        {
            // Optional override
        }

        // -------------------------------
        // ABSTRACT METHODS TO IMPLEMENT
        // -------------------------------
        protected abstract bool IsValidFuel(Pickupable item);
        protected abstract float GetChargeRate();
        protected abstract int GetCapacity();
        protected virtual bool ShouldConsumeFuel() => false;
        protected virtual bool IsFuelLocked() => false;

        public ItemsContainer GetContainer() => container;
    }
}
