using System.Collections.Generic;
using emt_sdk.Settings;
using UnityEngine;

public class ColorChangeComponent : MonoBehaviour
{
    public enum ColorChangeModeEnum
    {
        Brightness,
        BrightnessRed,
        BrightnessGreen,
        BrightnessBlue,
        Contrast,
        Saturation,
        CrossOver
    }

    // TODO: Testing moving shape during calibration

    public TransformCameraComponent Camera;

    // Active mode
    public ColorChangeModeEnum ColorChangeMode;

    public readonly Dictionary<ColorChangeModeEnum, float> DefaultValues = new Dictionary<ColorChangeModeEnum, float>
    {
        { ColorChangeModeEnum.Brightness, 1f },
        { ColorChangeModeEnum.BrightnessRed, 1f },
        { ColorChangeModeEnum.BrightnessGreen, 1f },
        { ColorChangeModeEnum.BrightnessBlue, 1f },
        { ColorChangeModeEnum.Contrast, 1f },
        { ColorChangeModeEnum.Saturation, 1f },
        { ColorChangeModeEnum.CrossOver, 0.0f },
    };

    public float Speed = 0.01f;
    public float ShiftMultiplier = 0.25f;
    public float RShiftStep = 0.001f;

    void SetActiveValue(float value)
    {
        switch (ColorChangeMode)
        {
            case ColorChangeModeEnum.Brightness:
                Camera.Setting.Color.Brightness = new ColorSetting.Color
                {
                    R = value,
                    G = value,
                    B = value
                };
                break;
            case ColorChangeModeEnum.BrightnessRed:
                Camera.Setting.Color.Brightness.R = value;
                break;
            case ColorChangeModeEnum.BrightnessGreen:
                Camera.Setting.Color.Brightness.G = value;
                break;
            case ColorChangeModeEnum.BrightnessBlue:
                Camera.Setting.Color.Brightness.B = value;
                break;
            case ColorChangeModeEnum.Contrast:
                Camera.Setting.Color.Contrast = value;
                break;
            case ColorChangeModeEnum.Saturation:
                Camera.Setting.Color.Saturation = value;
                break;
            case ColorChangeModeEnum.CrossOver:
                Camera.Setting.CrossOver = value;
                break;
        }
    }

    void AddActiveValue(float value, float min = -2f, float max = 2f)
    {
        var delta = Mathf.Clamp(value, min, max);

        switch (ColorChangeMode)
        {
            case ColorChangeModeEnum.Brightness:
                Camera.Setting.Color.Brightness.R += delta;
                Camera.Setting.Color.Brightness.G += delta;
                Camera.Setting.Color.Brightness.B += delta;
                
                Camera.Setting.Color.Brightness.R = Mathf.Clamp(Camera.Setting.Color.Brightness.R, -1f, 1f);
                Camera.Setting.Color.Brightness.G = Mathf.Clamp(Camera.Setting.Color.Brightness.G, -1f, 1f);
                Camera.Setting.Color.Brightness.B = Mathf.Clamp(Camera.Setting.Color.Brightness.B, -1f, 1f);
                break;
            case ColorChangeModeEnum.BrightnessRed:
                Camera.Setting.Color.Brightness.R += delta;
                Camera.Setting.Color.Brightness.R = Mathf.Clamp(Camera.Setting.Color.Brightness.R, -1f, 1f);
                break;
            case ColorChangeModeEnum.BrightnessGreen:
                Camera.Setting.Color.Brightness.G += delta;
                Camera.Setting.Color.Brightness.G = Mathf.Clamp(Camera.Setting.Color.Brightness.G, -1f, 1f);
                break;
            case ColorChangeModeEnum.BrightnessBlue:
                Camera.Setting.Color.Brightness.B += delta;
                Camera.Setting.Color.Brightness.B = Mathf.Clamp(Camera.Setting.Color.Brightness.B, -1f, 1f);
                break;
            case ColorChangeModeEnum.Contrast:
                Camera.Setting.Color.Contrast += delta;
                Camera.Setting.Color.Contrast = Mathf.Clamp(Camera.Setting.Color.Contrast, -1f, 1f);
                break;
            case ColorChangeModeEnum.Saturation:
                Camera.Setting.Color.Saturation += delta;
                Camera.Setting.Color.Saturation = Mathf.Clamp(Camera.Setting.Color.Saturation, -1f, 1f);
                break;
            case ColorChangeModeEnum.CrossOver:
                Camera.Setting.CrossOver += delta;
                Camera.Setting.CrossOver = Mathf.Clamp(Camera.Setting.CrossOver, 0f, 0.1f);
                break;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            var value = DefaultValues[ColorChangeMode];
            SetActiveValue(value);
            Camera.ApplySettings();
            return;
        }
        
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) ColorChangeMode = ColorChangeModeEnum.BrightnessRed;
        if (Input.GetKeyDown(KeyCode.G)) ColorChangeMode = ColorChangeModeEnum.BrightnessGreen;
        if (Input.GetKeyDown(KeyCode.B)) ColorChangeMode = ColorChangeModeEnum.BrightnessBlue;
        if (Input.GetKeyDown(KeyCode.J)) ColorChangeMode = ColorChangeModeEnum.Brightness;
        if (Input.GetKeyDown(KeyCode.C)) ColorChangeMode = ColorChangeModeEnum.Contrast;
        if (Input.GetKeyDown(KeyCode.S)) ColorChangeMode = ColorChangeModeEnum.Saturation;

        float speed = Time.deltaTime * Speed;
        float delta = 0f;
        
        if (Input.GetKey(KeyCode.LeftShift)) speed *= ShiftMultiplier;
        if (Input.GetKey(KeyCode.RightShift)) speed = RShiftStep;

        if (InputExtensions.GetKeyModified(KeyCode.KeypadPlus)) delta = speed;
        if (InputExtensions.GetKeyModified(KeyCode.KeypadMinus)) delta = -speed;

        if (InputExtensions.GetKeyModified(KeyCode.Equals)) delta = speed;
        if (InputExtensions.GetKeyModified(KeyCode.Minus)) delta = -speed;

        if (InputExtensions.GetKeyModified(KeyCode.Period)) delta = speed;
        if (InputExtensions.GetKeyModified(KeyCode.Comma)) delta = -speed;

        if (delta == 0) return;
        AddActiveValue(delta);
        Camera.ApplySettings();
    }
}
