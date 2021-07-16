using emt_sdk.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ProjectorColorSettingsComponent : MonoBehaviour
{
    // PlayerPrefs just ended up being a whole lot of spaghetti, may as well serialize it myself

    [Serializable]
    public class SliderSet
    {
        public Slider Saturation;
        public Slider Contrast;
        public Slider Brightness;

        public Slider SkewTop;
        public Slider SkewBottom;
    }

    public IPWSetting Setting { get; set; }

    public Slider LensShift;
    public SliderSet TopScreen;
    public SliderSet BottomScreen;

    public VolumeProfile TopProfile;
    public VolumeProfile BottomProfile;

    public DualCameraComponent Camera;

    public void LoadSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        if (!File.Exists(configFile))
        {
            Setting = new IPWSetting(); // Assume defaults
        }
        else
        {
            try
            {
                Setting = JsonConvert.DeserializeObject<IPWSetting>(File.ReadAllText(configFile));
            }
            catch (JsonException e)
            {
                Debug.Log(e);
                Setting = new IPWSetting(); // Loading failed, assume defaults
            }
        }

        Camera.Setting = Setting;

        LensShift.value = Setting.LensShift;

        TopScreen.Saturation.value = Setting.TopScreenColor.Saturation;
        TopScreen.Contrast.value = Setting.TopScreenColor.Contrast;
        TopScreen.Brightness.value = Setting.TopScreenColor.Brightness;
        TopScreen.SkewTop.value = Setting.TopScreenSkew.Top;
        TopScreen.SkewBottom.value = Setting.TopScreenSkew.Bottom;

        BottomScreen.Saturation.value = Setting.BottomScreenColor.Saturation;
        BottomScreen.Contrast.value = Setting.BottomScreenColor.Contrast;
        BottomScreen.Brightness.value = Setting.BottomScreenColor.Brightness;
        BottomScreen.SkewTop.value = Setting.BottomScreenSkew.Top;
        BottomScreen.SkewBottom.value = Setting.BottomScreenSkew.Bottom;
    }

    public void SaveSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        var json = JsonConvert.SerializeObject(Setting);

        File.WriteAllText(configFile, json);
    }

    public void ValueChanged()
    {
        UpdateValues();
        Camera.ApplySettings();
    }

    void Start()
    {
        LoadSettings();
        Camera.ApplySettings();
    }

    private void UpdateValues()
    {
        Setting.LensShift = LensShift.value;

        Setting.TopScreenColor.Saturation = TopScreen.Saturation.value;
        Setting.TopScreenColor.Contrast = TopScreen.Contrast.value;
        Setting.TopScreenColor.Brightness = TopScreen.Brightness.value;
        Setting.TopScreenSkew.Top = TopScreen.SkewTop.value;
        Setting.TopScreenSkew.Bottom = TopScreen.SkewBottom.value;

        Setting.BottomScreenColor.Saturation = BottomScreen.Saturation.value;
        Setting.BottomScreenColor.Contrast = BottomScreen.Contrast.value;
        Setting.BottomScreenColor.Brightness = BottomScreen.Brightness.value;
        Setting.BottomScreenSkew.Top = BottomScreen.SkewTop.value;
        Setting.BottomScreenSkew.Bottom = BottomScreen.SkewBottom.value;
    }
}
