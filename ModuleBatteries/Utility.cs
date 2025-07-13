using System.Collections;
using UnityEngine;
using Nautilus.Utility;

namespace ModuleBatteries.utilities
{
    public static class GameObjectExtensions
    {
        public static GameObject FindChild(this GameObject obj, string name)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == name)
                    return child.gameObject;
            }
            return null;
        }
        public static IEnumerator DisableMonitorScreen()
        {
            yield return new WaitForSeconds(2f); // Give game time to spawn it

            foreach (GameObject monitor in GameObject.FindObjectsOfType<GameObject>())
            {
                if (monitor.name.StartsWith("Wmonior(Clone)"))
                {
                    var screen = monitor.transform.Find("Starship_wall_monitor_01_screen");
                    if (screen != null)
                    {
                        Renderer renderer = screen.GetComponent<Renderer>();
                        if (renderer != null)
                        {
                            renderer.enabled = false; // Hides the screen texture only
                            Debug.Log("[MonitorPatch] Disabled screen on: " + monitor.name);
                        }
                    }
                }
            }
        }


     
       
        public static IEnumerator WaitAndLock(GameObject obj)
        {
            yield return new WaitForEndOfFrame();

            var storage = obj.GetComponentInChildren<StorageContainer>();
            if (storage != null)
            {
                LockStorage(storage);
            }
        }

        static void LockStorage(StorageContainer storage)
        {
            storage.container.isAllowedToRemove = (Pickupable pickupable, bool verbose) =>
            {
                ErrorMessage.AddMessage("You cannot remove items from this battery.");
                return false;
            };
        }

    }
  
}