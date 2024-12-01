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
    private const int DefaultResolution = 48;
    private const float DefaultCameraFps = 30f;
    private const float VisibilityCheckInterval = 1f;

    private float cameraFps = DefaultCameraFps;
    private float elapsedTime = 0f;

    private bool isMonitorVisible = true;
    private Transform cachedTargetTransform;
    private RenderTexture dummyTexture;

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
            targetPlayerTransform = allTargets[0].transform;
            cachedTargetTransform = targetPlayerTransform;
        }

        StartCoroutine(MonitorVisibilityAndDistanceCheck());
    }

    private void InitializeCamera()
    {
        renderTexture = new RenderTexture(DefaultResolution, DefaultResolution, 24);
        renderTexture.Create();

        dummyTexture = new RenderTexture(DefaultResolution, DefaultResolution, 24);
        dummyTexture.Create();

        helmetCamera = new GameObject("HelmetCamera").AddComponent<Camera>();
        helmetCamera.targetTexture = renderTexture;
        helmetCamera.clearFlags = CameraClearFlags.Depth;
        helmetCamera.backgroundColor = Color.black; // Set to black for reduced flickering
        helmetCamera.cullingMask = LayerMask.GetMask("Default");
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
                monitorRenderer.materials[2].mainTexture = dummyTexture; // Start with dummy texture
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

    private IEnumerator MonitorVisibilityAndDistanceCheck()
    {
        while (true)
        {
            if (monitorRenderer != null)
            {
                isMonitorVisible = monitorRenderer.isVisible;

                if (!isMonitorVisible || Vector3.Distance(Camera.main.transform.position, monitorObject.transform.position) > MaxRenderDistance)
                {
                    monitorRenderer.materials[2].mainTexture = dummyTexture; // Assign dummy texture when monitor not visible
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

        if (elapsedTime >= 1f / cameraFps)
        {
            elapsedTime = 0f;
            RenderCameraFrame();
        }
    }

    private void RenderCameraFrame()
    {
        helmetCamera.transform.position = cachedTargetTransform.position + new Vector3(0, 1.6f, 0);
        helmetCamera.transform.rotation = cachedTargetTransform.rotation;

        // Render the camera and update the monitor texture
        monitorRenderer.materials[2].mainTexture = renderTexture;
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
        }

        if (dummyTexture != null)
        {
            dummyTexture.Release();
        }

        if (helmetCamera != null)
        {
            Destroy(helmetCamera.gameObject);
        }
    }
}
