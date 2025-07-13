public class BatteryUpgradeEffect
{
    public TechType TechType;
    public int CapacityBonus;
    public float ChargeRateBonus;
    public bool GrantsToolBoost;

    public BatteryUpgradeEffect(TechType techType, int capacity = 0, float charge = 0f, bool toolBoost = false)
    {
        TechType = techType;
        CapacityBonus = capacity;
        ChargeRateBonus = charge;
        GrantsToolBoost = toolBoost;
    }
}