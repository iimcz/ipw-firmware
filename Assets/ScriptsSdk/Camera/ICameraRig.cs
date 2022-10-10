using UnityEngine;
using emt_sdk.ScenePackage;

public interface ICameraRig
{
    Rect GetBoundaries(float? distance = null);
    void ApplySettings();
    void SaveSettings();
    void SetBackgroundColor(Color color);
    void ShowSkybox();
    void SetViewport(Vector2 canvasSize, Viewport viewport);

    Naki3D.Common.Protocol.DeviceType DeviceType { get; }
    emt_sdk.Settings.IPWSetting.IPWOrientation Orientation { get; }
}
