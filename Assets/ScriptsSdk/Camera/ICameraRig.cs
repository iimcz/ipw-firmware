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

    emt_sdk.Settings.EMT.DeviceTypeEnum DeviceType { get; }
    emt_sdk.Settings.IPW.IPWSetting.IPWOrientation Orientation { get; }

    emt_sdk.Packages.CanvasDimensions DefaultCanvasDimensions { get; }
    Viewport DefaultViewport => new((int)DefaultCanvasDimensions.Width.Value, (int)DefaultCanvasDimensions.Height.Value, 0, 0);
}
