using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using ModuleBatteries.Components; // <- Your NuclearBatteryComponent
using ModuleBatteries.Items;
using Nautilus.Handlers;
using Nautilus.Utility;
using static ModuleBatteries.utilities.GameObjectExtensions;
using UnityEngine;
using UWE;

namespace ModuleBatteries;

public class NuclearBattery
{
    public static TechType techtype;
    public static void Register()
    {
        var info = PrefabInfo.WithTechType(
            "NuclearBattery",
            "Nuclear Battery",
            "Uses mini reactor rods to generate power."
        ).WithIcon(RamuneLib.Utils.ImageUtils.GetSprite("NuclearReactorBattery"));

        var battery = new CustomPrefab(info);

        // You can use CloneTemplate if you're cloning from a vanilla battery
        var template = new CloneTemplate(info, TechType.Battery); // or TechType.PowerCell
        battery.SetGameObject(template);

        template.ModifyPrefab += obj =>
        {
            obj.EnsureComponent<Battery>();
            obj.EnsureComponent<BatteryStorageController>();
            PrefabUtils.AddStorageContainer(obj, "StorageRoot", "Rod Insert", 4, 4);
            //UWE.CoroutineHost.StartCoroutine(WaitAndLock(obj));
            obj.EnsureComponent<NuclearBatteryComponent>();
        };
       
        battery.SetUnlock(TechType.PrecursorIonBattery);
        battery.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
        battery.SetRecipeFromJson(RamuneLib.Utils.JsonUtils.GetJsonRecipe("NuclearBatteryRecipe"))
            .WithFabricatorType(CraftTree.Type.Fabricator).WithStepsToFabricatorTab("Resources","Electronics");
        battery.Register();
        techtype = battery.Info.TechType;
    }
}