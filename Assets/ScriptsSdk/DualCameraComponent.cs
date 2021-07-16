using emt_sdk.Settings;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DualCameraComponent : MonoBehaviour
{
    public Camera TopCamera;
    public Camera BottomCamera;

    public VolumeProfile TopProfile;
    public VolumeProfile BottomProfile;

    public IPWSetting Setting;

    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }

    public void ApplySettings()
    {
        TopCamera.lensShift = new Vector2(0, Setting.LensShift);
        BottomCamera.lensShift = new Vector2(0, -Setting.LensShift);

        TopProfile.TryGet<ColorAdjustments>(out var colorTop);
        BottomProfile.TryGet<ColorAdjustments>(out var colorBottom);

        colorTop.postExposure.value = Setting.TopScreenColor.Brightness;
        colorTop.contrast.value = Setting.TopScreenColor.Contrast;
        colorTop.saturation.value = Setting.TopScreenColor.Saturation;

        colorBottom.postExposure.value = Setting.BottomScreenColor.Brightness;
        colorBottom.contrast.value = Setting.BottomScreenColor.Contrast;
        colorBottom.saturation.value = Setting.BottomScreenColor.Saturation;
    }
}
