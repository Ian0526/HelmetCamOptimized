using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(StartOfRound), "Start")]
public class HelmetCameraPatch
{
    private static GameObject helmetCameraObject;

    static void Postfix()
    {
        if (helmetCameraObject == null)
        {
            // Ensure only one instance of the behavior exists
            helmetCameraObject = new GameObject("HelmetCameraBehavior");
            helmetCameraObject.AddComponent<HelmetCameraBehavior>();
            Object.DontDestroyOnLoad(helmetCameraObject);

            Debug.Log("[HelmetCamsOptimized] HelmetCameraBehavior added to the scene.");
        }
    }
}
