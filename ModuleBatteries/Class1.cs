using System.Reflection;
using BepInEx;
using HarmonyLib;
using ModuleBatteries.Items;
using ModuleBatteries.Patches;
using UWE;
using static ModuleBatteries.utilities.GameObjectExtensions;
using static ModuleBatteries.text.MonitorOverlayManager;

namespace ModuleBatteries
{
    [BepInPlugin("com.mediccookie.modulebatteries", "Module Batteries", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmonyInstance = new Harmony("Cookie.subnautica.ModuleBatteries.mod");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            // Initialize your battery types here
            Logger.LogInfo("[ModuleBatteries] Plugin loaded.");

            NuclearBattery.Register();
            BioReactorBattery.Register();
            ReactorRodItems.Register();
            ModuleBattery.Register(); // if needed
          AddBatteryChargerCompatibility(ModuleBattery.techtype);
          AddBatteryChargerCompatibility(NuclearBattery.techtype);
          AddBatteryChargerCompatibility(BioReactorBattery.techtype);
          ChargerPatcher.Patch(harmonyInstance);
         
            Logger.LogInfo("[ModuleBatteries] Batteries registered successfully.");
        }

        private void Start()
        {
            CoroutineHost.StartCoroutine(DisableMonitorScreen());
            CoroutineHost.StartCoroutine(ApplyOverlayToAllMonitors());
           
        }

        private void AddBatteryChargerCompatibility(TechType techType)
        {
            if (!BatteryCharger.compatibleTech.Contains(techType))
                BatteryCharger.compatibleTech.Add(techType);
        }


    }
}