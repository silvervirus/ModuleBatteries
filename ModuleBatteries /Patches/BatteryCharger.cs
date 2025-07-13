using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static ModuleBatteries.utilities.GameObjectExtensions;
using static Charger;

namespace ModuleBatteries.Patches
{
    internal static class ChargerPatcher
    {
        internal static void Patch(Harmony harmony)
        {
            var onEquip = AccessTools.Method(typeof(Charger), nameof(Charger.OnEquip));
            var postfix = AccessTools.Method(typeof(ChargerPatcher), nameof(OnEquipPostfix));

            harmony.Patch(onEquip, postfix: new HarmonyMethod(postfix));
        }

        private static void OnEquipPostfix(Charger __instance, string slot, InventoryItem item, Dictionary<string, SlotDefinition> ___slots)
        {
            if (!___slots.TryGetValue(slot, out SlotDefinition slotDef))
                return;

            GameObject batterySlotModel = slotDef.battery;
            if (batterySlotModel == null || item?.item == null)
                return;

            GameObject insertedModel = null;

            if (__instance is BatteryCharger)
            {
                insertedModel = item.item.transform.Find("model/battery_01")?.gameObject
                             ?? item.item.transform.Find("model/battery_ion")?.gameObject;
            }
            else if (__instance is PowerCellCharger)
            {
                GameObject go = item.item.gameObject;
                insertedModel = go.transform.Find("model/engine_power_cell_01")?.gameObject
                                           ?? go.transform.Find("model/engine_power_cell_ion")?.gameObject
                                           ?? go.FindChild("engine_power_cell_01")
                                           ?? go.FindChild("engine_power_cell_ion");
            }
            if (insertedModel == null)
                return;

            if (insertedModel.TryGetComponent(out MeshFilter modelMesh) &&
                batterySlotModel.TryGetComponent(out MeshFilter chargerMesh))
            {
                chargerMesh.mesh = modelMesh.mesh;
            }

            if (insertedModel.TryGetComponent(out Renderer modelRenderer) &&
                batterySlotModel.TryGetComponent(out Renderer chargerRenderer))
            {
                chargerRenderer.material.CopyPropertiesFromMaterial(modelRenderer.material);
            }
        }
    }
}
