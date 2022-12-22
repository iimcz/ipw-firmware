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
    
    Vector2 SyncLensShift { get; set; }

    Vector2 CanvasDimensions { get; }
    Viewport Viewport { get; }

    Naki3D.Common.Protocol.DeviceType DeviceType { get; }
    emt_sdk.Settings.IPWSetting.IPWOrientation Orientation { get; }

    emt_sdk.Generated.ScenePackage.CanvasDimensions DefaultCanvasDimensions { get; }
    Viewport DefaultViewport => new((int)DefaultCanvasDimensions.Width.Value, (int)DefaultCanvasDimensions.Height.Value, 0, 0);
}
