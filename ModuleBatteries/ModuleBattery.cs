using ModuleBatteries.Components;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using static ModuleBatteries.utilities.GameObjectExtensions;
using UWE;

namespace ModuleBatteries;

public class ModuleBattery
{
    public static TechType techtype;
    public static void Register()
    {
        var info = PrefabInfo.WithTechType(
            "ModuleBatteries",
            "Module Battery",
            "Uses Bio Resources to create modifications to the battery ."
        ).WithIcon(RamuneLib.Utils.ImageUtils.GetSprite("ModuleBattery"));

        var battery = new CustomPrefab(info);

        // You can use CloneTemplate if you're cloning from a vanilla battery
        var template = new CloneTemplate(info, TechType.Battery); // or TechType.PowerCell
        battery.SetGameObject(template);

        template.ModifyPrefab += obj =>
        {
            var batteryComponent = obj.EnsureComponent<Battery>();
            batteryComponent._capacity = 100;
            batteryComponent.charge = 5;
            obj.EnsureComponent<BatteryStorageController>();
            PrefabUtils.AddStorageContainer(obj, "StorageRoot", "Module Object Insert", 4, 4);
           // UWE.CoroutineHost.StartCoroutine(WaitAndLock(obj));
            obj.EnsureComponent<ModuleBatteryComponent>();
        };

        battery.SetUnlock(TechType.PrecursorIonBattery);
        battery.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
        battery.SetRecipeFromJson(RamuneLib.Utils.JsonUtils.GetJsonRecipe("ModuleBatteryRecipe"))
            .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Resources","Electronics");
        battery.Register();
        techtype = battery.Info.TechType;
    }
}
