using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModuleBatteries.text
{
    public static class MonitorOverlayManager
    {
        public static IEnumerator ApplyOverlayToAllMonitors()
        {
            yield return new WaitForSeconds(2f); // wait for monitors to spawn

            foreach (GameObject monitor in GameObject.FindObjectsOfType<GameObject>())
            {
                if (!monitor.name.StartsWith("Wmonior"))
                    continue;

                Transform screen = monitor.transform.Find("Starship_wall_monitor_01_screen");
                if (screen == null)
                {
                    Debug.LogWarning($"[MonitorOverlay] Could not find screen in {monitor.name}");
                    continue;
                }

                Renderer renderer = screen.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material[] mats = renderer.materials;
                    if (mats.Length > 1)
                    {
                        // Disable screen texture on material 1
                        mats[1].mainTexture = null;
                        mats[1].color = Color.black;
                        renderer.materials = mats;

                        Debug.Log($"[MonitorOverlay] Cleared material 1 on {screen.name} under {monitor.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[MonitorOverlay] Not enough materials on {screen.name}");
                    }
                }

                AddLabelTo(screen.gameObject);
            }
        }

        private static void AddLabelTo(GameObject target)
        {
            GameObject canvasObj = new GameObject("MonitorOverlayCanvas");
            canvasObj.transform.SetParent(target.transform, false);
            canvasObj.transform.localPosition = new Vector3(0f, 0f, 0.01f);
            canvasObj.transform.localRotation = Quaternion.identity;

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.scaleFactor = 100;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 10;

            canvasObj.AddComponent<GraphicRaycaster>().enabled = false;

            GameObject textGO = new GameObject("MonitorText");
            textGO.transform.SetParent(canvasObj.transform, false);

            var text = textGO.AddComponent<TextMeshProUGUI>();
            text.text = "SYSTEM ONLINE"; // customize as needed
            text.fontSize = 5f;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.green;

            var rt = text.rectTransform;
            rt.sizeDelta = new Vector2(200f, 100f);
            rt.localPosition = Vector3.zero;
        }
    }
}
