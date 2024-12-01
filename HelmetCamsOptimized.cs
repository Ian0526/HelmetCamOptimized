using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

[BepInPlugin("Ovchinikov.HelmetCamsOptimized.Main", "HelmetCamsOptimized", "1.0.0")]
public class HelmetCamsOptimizedMain : BaseUnityPlugin
{
    private static ManualLogSource logger;

    private void Awake()
    {
        logger = Logger;
        logger.LogInfo("HelmetCamsOptimized plugin loaded.");

        // Initialize and patch Netcode
        NetcodePatcher();

        // Apply Harmony patches
        var harmony = new Harmony("Ovchinikov.HelmetCamsOptimized");
        harmony.PatchAll();

        // Add HelmetCameraBehavior
        var helmetCameraObject = new GameObject("HelmetCameraBehavior");
        helmetCameraObject.AddComponent<HelmetCameraBehavior>();

        logger.LogInfo("HelmetCameraBehavior added to the game.");
    }

    public static void Log(string message)
    {
        logger?.LogInfo(message);
    }

    private static void NetcodePatcher()
    {
        Log("[NetcodePatcher] Invoking runtime patches...");
        var types = Assembly.GetExecutingAssembly().GetTypes();

        foreach (var type in types)
        {
            var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                if (attributes.Length > 0)
                {
                    try
                    {
                        method.Invoke(null, null);
                        Log($"[NetcodePatcher] Successfully invoked {method.Name} in {type.FullName}.");
                    }
                    catch (System.Exception ex)
                    {
                        Log($"[NetcodePatcher] Failed to invoke {method.Name} in {type.FullName}: {ex.Message}");
                    }
                }
            }
        }
        Log("[NetcodePatcher] Completed runtime initialization.");
    }

    private void OnDestroy()
    {
        Log("[HelmetCamsOptimizedMain] Plugin is being unloaded.");
    }
}
