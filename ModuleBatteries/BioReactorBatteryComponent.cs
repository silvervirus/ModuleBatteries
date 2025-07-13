using ModuleBatteries.Behaviours;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BioReactorBatteryComponent : CustomBatteryBase
{
    // ✅ Optional fallback energy values if BaseBioReactor.charge fails
    private static readonly Dictionary<TechType, float> fallbackFishEnergy = new()
    {
        { TechType.Peeper, 200f },
        { TechType.Bladderfish, 150f },
        { TechType.HoleFish, 180f },
        { TechType.GarryFish, 160f },
        { TechType.Spadefish, 190f },
        { TechType.Eyeye, 175f },
        { TechType.Boomerang, 170f },
        { TechType.Oculus, 250f },
    };

    protected override float GetChargeRate() => 0f; // fuel-based only
    protected override int GetCapacity() => 500;
    protected override bool IsFuelLocked() => true;
    protected override bool ShouldConsumeFuel() => true;
    protected override void CreateContainer()
    {
        container = new ItemsContainer(
            InventoryDimensions.x,
            InventoryDimensions.y,
            transform,
            string.Empty,
            null
        );

        container.isAllowedToAdd = (pickupable, verbose) =>
        {
            if (pickupable == null)
                return false;

            TechType tech = pickupable.GetTechType();
            bool isAllowed = BaseBioReactor.charge.ContainsKey(tech) || fallbackFishEnergy.ContainsKey(tech);

            if (!isAllowed && verbose)
                ErrorMessage.AddMessage($"{Language.main.Get(tech)} cannot be used as BioFuel!");

            return isAllowed;
        };

        container.onAddItem += OnItemAdded;
        container.onRemoveItem += OnItemRemoved;
    }

    protected override bool IsValidFuel(Pickupable item)
    {
        if (item == null) return false;
        var tech = item.GetTechType();
        return BaseBioReactor.charge.ContainsKey(tech) || fallbackFishEnergy.ContainsKey(tech);
    }

    protected override void TryConsumeFuel()
    {
        foreach (var item in container)
        {
            if (item?.item == null)
                continue;

            var tech = item.item.GetTechType();
            float totalEnergy;

            // ✅ Use BaseBioReactor values if available, fallback otherwise
            if (!BaseBioReactor.charge.TryGetValue(tech, out totalEnergy) &&
                !fallbackFishEnergy.TryGetValue(tech, out totalEnergy))
            {
                Debug.Log($"[BioBattery] Skipping invalid fuel: {tech}");
                continue;
            }

            var tag = item.item.GetComponent<FuelTag>();
            if (tag == null)
            {
                tag = item.item.gameObject.AddComponent<FuelTag>();
                tag.remainingEnergy = totalEnergy;
                Debug.Log($"[BioBattery] Assigned {totalEnergy} energy to {tech}");
            }

            float chargePerTick = 2f;
            float toAdd = Mathf.Min(chargePerTick, tag.remainingEnergy);

            battery.charge = Mathf.Min(battery.capacity, battery.charge + toAdd);
            tag.remainingEnergy -= toAdd;

            Debug.Log($"[BioBattery] +{toAdd} charge from {tech}, RemainingFuel: {tag.remainingEnergy}");

            if (tag.remainingEnergy <= 0f)
            {
                container.RemoveItem(item.item, true);
                Debug.Log($"[BioBattery] {tech} fully consumed and removed");
            }

            break; // Only one item per tick
        }
    }
}
