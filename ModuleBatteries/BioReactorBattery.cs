using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using ModuleBatteries.Components; // <- Your NuclearBatteryComponent
using ModuleBatteries.Items;
using Nautilus.Handlers;
using static ModuleBatteries.utilities.GameObjectExtensions;
using Nautilus.Utility;
using UnityEngine;

namespace ModuleBatteries;

public class BioReactorBattery
{
    public static TechType techtype;
    public static void Register()
    {
        var info = PrefabInfo.WithTechType(
            "BioRectBattery",
            "Bio Reactor Battery",
            "Uses Living Resources into energy to generate power."
        ).WithIcon(RamuneLib.Utils.ImageUtils.GetSprite("BioRectBattery"));

        var battery = new CustomPrefab(info);

        // You can use CloneTemplate if you're cloning from a vanilla battery
        var template = new CloneTemplate(info, TechType.Battery); // or TechType.PowerCell
        battery.SetGameObject(template);

        template.ModifyPrefab += obj =>
        {
            var batteryComponent = obj.EnsureComponent<Battery>();
            batteryComponent._capacity = 800;
            batteryComponent.charge = 100;
            obj.EnsureComponent<BatteryStorageController>();
            PrefabUtils.AddStorageContainer(obj, "StorageRoot", "Bio Reactor Battery Tank", 4, 4);
            //UWE.CoroutineHost.StartCoroutine(WaitAndLock(obj));
            obj.EnsureComponent<BioReactorBatteryComponent>();
        };
     
        battery.SetUnlock(TechType.Seamoth);
        battery.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
        battery.SetRecipeFromJson(RamuneLib.Utils.JsonUtils.GetJsonRecipe("BioReactorBatteryRecipe"))
            .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Resources","Electronics");
            
        battery.Register();
        techtype = battery.Info.TechType; 
    }
}