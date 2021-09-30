using emt_sdk.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class DualCameraComponent : MonoBehaviour
{
    [SceneObjectsOnly, Required, BoxGroup("Rendering")]
    public TransformCameraComponent TopCamera;
    [SceneObjectsOnly, Required, BoxGroup("Rendering")]
    public TransformCameraComponent BottomCamera;
    [OnValueChanged("OrientationChanged"), BoxGroup("Rendering")]
    public IPWSetting.IPWOrientation Orientation;

    public IPWSetting Setting;

    private void OrientationChanged()
    {
        Setting.Orientation = Orientation;
        ApplySettings();
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
        yield return new WaitForSecondsRealtime(1f);
        ApplySettings();
    }

    [Button, BoxGroup("Settings")]
    public void LoadSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        try
        {
            var json = File.ReadAllText(configFile);
            Setting = JsonConvert.DeserializeObject<IPWSetting>(json);

            TopCamera.Setting = Setting.Displays[0];
            BottomCamera.Setting = Setting.Displays[1];
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Setting = new IPWSetting(); // Loading failed, assume defaults
            Setting.Displays = new List<DisplaySetting>
            {
                new DisplaySetting(),
                new DisplaySetting()
            };

            TopCamera.Setting = Setting.Displays[0]; // Duplicated since we can throw on missing info before
            BottomCamera.Setting = Setting.Displays[1];

            TopCamera.TargetDisplay = 0;
            BottomCamera.TargetDisplay = 1;
        }

        Orientation = Setting.Orientation;
    }

    public Rect GetBoundaries()
    {
        var topBoundary = TopCamera.GetCameraBoundries();
        var bottomBoundary = BottomCamera.GetCameraBoundries();
        var size = new Vector2(bottomBoundary.xMax - topBoundary.xMax, topBoundary.yMax); // Up / down shift only

        return new Rect(topBoundary.x, topBoundary.y, size.x, size.y);
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
    }

    [Button, LabelText("Swap Displays"), BoxGroup("Rendering")]
    public void Swap()
    {
        var firstDisplay = TopCamera.TargetDisplay;
        var secondDisplay = BottomCamera.TargetDisplay;

        TopCamera.TargetDisplay = secondDisplay;
        BottomCamera.TargetDisplay = firstDisplay;
    }
}
