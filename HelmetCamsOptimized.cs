using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using UnityEngine;

[BepInPlugin("Ovchinikov.HelmetCamsOptimized.Main", "HelmetCamsOptimized", "1.0.0")]
[BepInDependency("RickArg.lethalcompany.helmetcameras", BepInDependency.DependencyFlags.SoftDependency)]
public class HelmetCamsOptimizedMain : BaseUnityPlugin
{
    private static ManualLogSource logger;

    private void Awake()
    {
        logger = Logger;
        logger.LogInfo("HelmetCamsOptimized plugin loaded.");

        // if other plugin is present, force disable it
        CheckAndDisableOldPlugin();

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

    private void CheckAndDisableOldPlugin()
    {
        string oldPluginGUID = "RickArg.lethalcompany.helmetcameras";

        // Look for the old plugin in the loaded plugin chain
        var oldPluginInfo = BepInEx.Bootstrap.Chainloader.PluginInfos
            .FirstOrDefault(p => p.Value.Metadata.GUID == oldPluginGUID);

        if (oldPluginInfo.Value != null)
        {
            Logger.LogWarning($"Detected old HelmetCams plugin: {oldPluginInfo.Value.Metadata.Name} v{oldPluginInfo.Value.Metadata.Version}");

            // Locate and disable its core MonoBehaviour (if any)
            var oldPluginInstance = oldPluginInfo.Value.Instance as MonoBehaviour;
            if (oldPluginInstance != null)
            {
                oldPluginInstance.enabled = false;
                Logger.LogInfo("Old HelmetCams MonoBehaviour disabled. You can remove the old plugin from your modpack.");
            }
        }
    }

    private void OnDestroy()
    {
        Log("[HelmetCamsOptimizedMain] Plugin is being unloaded.");
    }
}
