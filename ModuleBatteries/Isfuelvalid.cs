using System.Collections.Generic;
using UnityEngine;

public static class FuelValidationHelpers
{
    private static readonly HashSet<TechType> fishTechTypes = new()
    {
        TechType.Peeper,
        TechType.Boomerang,
        TechType.GarryFish,
        TechType.HoleFish,
        TechType.Hoopfish,
        TechType.Spadefish,
        TechType.Eyeye,
        TechType.Bladderfish,
        TechType.Oculus,
        TechType.Reginald,
        TechType.Spinefish
      
      
    };

    public static bool IsFishTech(this TechType techType)
    {
        return fishTechTypes.Contains(techType);
    }

    public static bool IsFishPickup(this Pickupable pickupable)
    {
        return pickupable != null && IsFishTech(pickupable.GetTechType());
    }
}