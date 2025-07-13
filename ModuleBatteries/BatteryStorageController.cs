using System;
using System.IO;
using System.Reflection;
using ModuleBatteries;
using UnityEngine;

public class BatteryStorageController : MonoBehaviour
{
    private KeyCode openKey = KeyCode.G;
    private StorageContainer currentlyOpenStorage;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // ensure persistence
    }

    private void Start()
    {
        string configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "battery_storage_config.json");
        if (File.Exists(configPath))
        {
            var config = BatteryStorageConfig.Load(configPath);
            if (Enum.TryParse(config.OpenBatteryStorageKey, true, out KeyCode key))
                openKey = key;
        }
    }

    private void Update()
    {
        if (Player.main == null || Inventory.main == null)
            return;

        // Close previously opened battery storage if PDA is closed
        PDA playerPDA = Player.main?.GetPDA();
        if (currentlyOpenStorage != null && (playerPDA == null || !playerPDA.isOpen))
        {
            currentlyOpenStorage.open = false;
            currentlyOpenStorage = null;
        }

        if (Input.GetKeyDown(openKey))
        {
            // 1. Held object (either a battery or a tool)
            GameObject held = Inventory.main.GetHeldObject();
            if (TryOpenBatteryStorage(held))
                return;

            // 2. Check tool's battery slot
            if (held != null && held.TryGetComponent(out EnergyMixin energy))
            {
                var battery = energy.GetBattery();
                if (battery != null && battery is MonoBehaviour mb && TryOpenBatteryStorage(mb.gameObject))
                    return;
            }

            // 3. Hovered or dragged inventory item
            InventoryItem hoveredItem = ItemDragManager.hoveredItem;
            if (hoveredItem != null && TryOpenBatteryStorage(hoveredItem.item.gameObject))
                return;

          

            ErrorMessage.AddMessage("No valid battery selected, hovered, or dragged.");
        }
    }

    private bool TryOpenBatteryStorage(GameObject obj)
    {
        if (obj == null)
            return false;

        TechType techType = CraftData.GetTechType(obj);
        if ((techType == NuclearBattery.techtype ||
             techType == ModuleBattery.techtype ||
             techType == BioReactorBattery.techtype) &&
            obj.TryGetComponent(out StorageContainer storage))
        {
            storage.Open();
            currentlyOpenStorage = storage;
            return true;
        }

        return false;
    }

    private bool TryOpenStorageFromHeldTool(GameObject heldTool)
    {
        if (heldTool == null)
            return false;

        Transform batterySlot = heldTool.transform.Find("BatterySlot");
        if (batterySlot == null || batterySlot.childCount == 0)
            return false;

        Transform batteryObj = batterySlot.GetChild(0);
        if (batteryObj == null)
            return false;

        GameObject batteryGO = batteryObj.gameObject;

        TechType techType = CraftData.GetTechType(batteryGO);
        if ((techType == NuclearBattery.techtype ||
             techType == ModuleBattery.techtype ||
             techType == BioReactorBattery.techtype) &&
            batteryGO.TryGetComponent(out StorageContainer storage))
        {
            storage.Open();
            currentlyOpenStorage = storage;
            return true;
        }

        return false;
    }
}
