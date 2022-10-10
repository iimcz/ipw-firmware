using emt_sdk.Settings;
using emt_sdk.ScenePackage;
using UnityEngine;

public class PeppersGhostCameraComponent : MonoBehaviour, ICameraRig
{
    Naki3D.Common.Protocol.DeviceType ICameraRig.DeviceType => Naki3D.Common.Protocol.DeviceType.Pge;

    public IPWSetting.IPWOrientation Orientation => IPWSetting.IPWOrientation.Single;

    public Camera Camera;

    void Awake()
    {
        Display.displays[0].SetRenderingResolution(2048, 2048);
        Camera.aspect = 1;
    }

    public Rect GetBoundaries(float? distance = null)
    {
        var dist = distance ?? Camera.nearClipPlane;
        var bottomLeft = Camera.ViewportToWorldPoint(new Vector3(0, 0, dist));
        var topRight = Camera.ViewportToWorldPoint(new Vector3(1, 1, dist));

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    public void ApplySettings()
    {
        // No settings to apply
    }

    public void SaveSettings()
    {
        // Nothing to save
    }

    public void SetBackgroundColor(Color color)
    {
        Camera.clearFlags = CameraClearFlags.SolidColor;
        Camera.backgroundColor = color;
    }

    public void ShowSkybox()
    {
        Camera.clearFlags = CameraClearFlags.Skybox;
    }

    public void SetViewport(Vector2 canvasSize, Viewport viewport)
    {
        // Nothing to apply, always fixed
    }
}
