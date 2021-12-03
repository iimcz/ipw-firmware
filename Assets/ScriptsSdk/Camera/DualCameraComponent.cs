using emt_sdk.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

public class DualCameraComponent : MonoBehaviour
{
    [SceneObjectsOnly, Required, BoxGroup("Rendering")]
    public TransformCameraComponent TopCamera;
    [SceneObjectsOnly, Required, BoxGroup("Rendering")]
    public TransformCameraComponent BottomCamera;
    [OnValueChanged("OrientationChanged"), BoxGroup("Rendering")]
    public IPWSetting.IPWOrientation Orientation;

    public IPWSetting Setting;

    [OnValueChanged("CurveChanged"), LabelText("Brightness curve on"), BoxGroup("Debug")]
    public bool CurveOn = true;

    // TODO: Fix ortho sizing, lens shift is just physical worldspace shift scaled by orthosize
    // TODO: Disable culling on ortho scenes, leaves black squares for some reason

    private void OrientationChanged()
    {
        Setting.Orientation = Orientation;
        ApplySettings();
    }

    void CurveChanged()
    {
        ProjectorTransformationPass.EnableCurve = CurveOn;
    }

    void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            if (Display.displays[i].active) continue;
            Display.displays[i].Activate();
        }

        LoadSettings();
        StartCoroutine(ApplyDelay());
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        ApplySettings();
    }

    [Button, BoxGroup("Settings")]
    public void LoadSettings()
    {
        Setting = ProjectorTransfomartionSettingsLoader.LoadSettings();

        TopCamera.Settings = Setting.Displays;
        BottomCamera.Settings = Setting.Displays;
        Orientation = Setting.Orientation;
    }

    public Rect GetBoundaries(float? distance = null)
    {
        if (Setting.Orientation == IPWSetting.IPWOrientation.Single)
            return distance.HasValue ? TopCamera.GetCameraBoundries(distance.Value) : TopCamera.GetCameraBoundries();
        
        // Right in horizontal
        var topBoundary = distance.HasValue ? 
            TopCamera.GetCameraBoundries(distance.Value) : 
            TopCamera.GetCameraBoundries();
        
        // Left in horizontal
        var bottomBoundary = distance.HasValue ? 
            BottomCamera.GetCameraBoundries(distance.Value) : 
            BottomCamera.GetCameraBoundries();

        Vector2 size;
        switch (Setting.Orientation)
        {
            case IPWSetting.IPWOrientation.Vertical:
                size = new Vector2(bottomBoundary.width, bottomBoundary.yMax - topBoundary.y);
                return new Rect(topBoundary.x, topBoundary.y, size.x, size.y);
            case IPWSetting.IPWOrientation.Horizontal:
                size = new Vector2(topBoundary.xMax - bottomBoundary.x, bottomBoundary.height);
                return new Rect(bottomBoundary.x, bottomBoundary.y, size.x, size.y);
            default:
                throw new NotSupportedException();
        }
    }

    [Button, BoxGroup("Settings")]
    public void ApplySettings()
    {
        switch (Setting.Orientation)
        {
            case IPWSetting.IPWOrientation.Vertical:
                TopCamera.Camera.lensShift = new Vector2(0, Setting.LensShift);
                BottomCamera.Camera.lensShift = new Vector2(0, -Setting.LensShift);
                break;
            case IPWSetting.IPWOrientation.Horizontal:
                TopCamera.Camera.lensShift = new Vector2(Setting.LensShift, 0);
                BottomCamera.Camera.lensShift = new Vector2(-Setting.LensShift, 0);
                break;
            case IPWSetting.IPWOrientation.Single:
                TopCamera.Camera.lensShift = Vector2.zero;
                BottomCamera.Camera.lensShift = Vector2.zero;
                break;
            default:
                throw new NotImplementedException();
        }

        if (TopCamera.Camera.orthographic)
        {
            TopCamera.gameObject.transform.position = new Vector3(TopCamera.Camera.orthographicSize - Setting.LensShift, 0, 0);
            BottomCamera.gameObject.transform.position = new Vector3(-BottomCamera.Camera.orthographicSize + Setting.LensShift, 0, 0);
        }

        TopCamera.ApplySettings();
        BottomCamera.ApplySettings();
    }

    [Button, BoxGroup("Settings")]
    public void SaveSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        var json = JsonConvert.SerializeObject(Setting, Formatting.Indented);

        File.WriteAllText(configFile, json);
        Debug.Log("Configuration saved");
    }

    [Button, LabelText("Swap Settings"), BoxGroup("Rendering")]
    public void SwapSettings()
    {
        (TopCamera.SettingIndex, BottomCamera.SettingIndex) = (BottomCamera.SettingIndex, TopCamera.SettingIndex);
    }

    [Button, LabelText("Swap Displays"), BoxGroup("Rendering")]
    public void SwapDisplays()
    {
        var firstDisplay = TopCamera.TargetDisplay;
        var secondDisplay = BottomCamera.TargetDisplay;

        TopCamera.TargetDisplay = secondDisplay;
        BottomCamera.TargetDisplay = firstDisplay;
    }
}
