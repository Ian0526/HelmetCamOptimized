using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelmetCameraBehavior : MonoBehaviour
{
    private RenderTexture renderTexture;
    private Camera helmetCamera;
    private GameObject monitorObject;
    private MeshRenderer monitorRenderer;

    private List<TransformAndName> allTargets = new List<TransformAndName>();
    private Transform targetPlayerTransform;

    private const float MaxRenderDistance = 50f;
    private const int DefaultResolution = 1024;
    private const float DefaultCameraFps = 30f;
    private const float VisibilityCheckInterval = 1f;

    private float cameraFps = DefaultCameraFps;
    private float elapsedTime = 0f;

    private bool isMonitorVisible = true;
    private Transform cachedTargetTransform;

    private void Awake()
    {
        StartCoroutine(InitializeAfterDelay());
    }

    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(5); // Allow the scene to fully load

        InitializeCamera();
        AssignMonitorTexture();
        DisableVanillaCamera();
        UpdateTargets();

        if (allTargets.Count > 0)
        {
            SetTarget(allTargets[0]);
        }

        StartCoroutine(MonitorVisibilityCheck());
    }

    private void InitializeCamera()
    {
        renderTexture = new RenderTexture(DefaultResolution, DefaultResolution, 24);
        renderTexture.Create();

        helmetCamera = new GameObject("HelmetCamera").AddComponent<Camera>();
        helmetCamera.targetTexture = renderTexture;
        helmetCamera.cullingMask = 20649983;
        helmetCamera.farClipPlane = MaxRenderDistance;
        helmetCamera.nearClipPlane = 0.55f;

        DontDestroyOnLoad(helmetCamera.gameObject);
    }

    private void AssignMonitorTexture()
    {
        monitorObject = GameObject.Find("Environment/HangarShip/ShipModels2b/MonitorWall/Cube.001");

        if (monitorObject != null)
        {
            monitorRenderer = monitorObject.GetComponent<MeshRenderer>();
            if (monitorRenderer != null)
            {
                monitorRenderer.materials[2].mainTexture = renderTexture; // Ensure only `renderTexture` is applied
            }
        }
    }

    private void DisableVanillaCamera()
    {
        GameObject vanillaCameraObject = GameObject.Find("Environment/HangarShip/Cameras/ShipCamera");

        if (vanillaCameraObject != null)
        {
            Camera vanillaCamera = vanillaCameraObject.GetComponent<Camera>();
            if (vanillaCamera != null)
            {
                vanillaCamera.enabled = false;
            }

            vanillaCameraObject.SetActive(false);
        }
    }

    private void UpdateTargets()
    {
        allTargets.Clear();
        foreach (var player in StartOfRound.Instance.allPlayerScripts)
        {
            allTargets.Add(new TransformAndName(player.transform, player.playerUsername));
        }
    }

    private void SetTarget(TransformAndName target)
    {
        var helmetLights = target.transform.Find("ScavengerModel/metarig/CameraContainer/MainCamera/HelmetLights");
        if (helmetLights != null)
        {
            cachedTargetTransform = helmetLights;
        }
        else
        {
            cachedTargetTransform = target.transform; // Fallback if "HelmetLights" isn't found
        }
    }

    private IEnumerator MonitorVisibilityCheck()
    {
        while (true)
        {
            if (monitorRenderer != null)
            {
                isMonitorVisible = monitorRenderer.isVisible;

                // Disable rendering when the monitor is out of range or not visible
                if (!isMonitorVisible)
                {
                    yield return new WaitForSeconds(VisibilityCheckInterval);
                    continue;
                }
            }

            yield return null;
        }
    }

    private void LateUpdate()
    {
        if (!isMonitorVisible || cachedTargetTransform == null || helmetCamera == null)
            return;

        elapsedTime += Time.deltaTime;

        if (elapsedTime > 1f / cameraFps)
        {
            elapsedTime = 0f;
            RenderCameraFrame();
        }
    }

    private void RenderCameraFrame()
    {
        if (cachedTargetTransform == null)
            return;

        // Update the camera's position and rotation based on the player's "HelmetLights" transform
        helmetCamera.transform.position = cachedTargetTransform.position;
        helmetCamera.transform.rotation = cachedTargetTransform.rotation;

        // Ensure the monitor is using the correct texture
        if (monitorRenderer != null && monitorRenderer.materials[2].mainTexture != renderTexture)
        {
            monitorRenderer.materials[2].mainTexture = renderTexture;
        }
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (helmetCamera != null)
        {
            Destroy(helmetCamera.gameObject);
        }
    }
}
