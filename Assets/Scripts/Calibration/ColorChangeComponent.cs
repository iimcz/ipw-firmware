using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeComponent : MonoBehaviour
{
    public enum ColorChangeModeEnum
    {
        Brightness,
        Contrast,
        Saturation,
        CrossOver
    }

    // TODO: Testing moving shape during calibration

    [Required, SceneObjectsOnly]
    public TransformCameraComponent Camera;

    [LabelText("Active mode")]
    public ColorChangeModeEnum ColorChangeMode;

    public readonly Dictionary<ColorChangeModeEnum, float> DefaultValues = new Dictionary<ColorChangeModeEnum, float>
    {
        { ColorChangeModeEnum.Brightness, 0f },
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
                Camera.Setting.Color.Brightness = value;
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
                Camera.Setting.Color.Brightness += delta;
                Camera.Setting.Color.Brightness = Mathf.Clamp(Camera.Setting.Color.Brightness, -1f, 1f);
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
        if (Input.GetKeyDown(KeyCode.B)) ColorChangeMode = ColorChangeModeEnum.Brightness;
        if (Input.GetKeyDown(KeyCode.C)) ColorChangeMode = ColorChangeModeEnum.Contrast;
        if (Input.GetKeyDown(KeyCode.S)) ColorChangeMode = ColorChangeModeEnum.Saturation;
        //if (Input.GetKeyDown(KeyCode.O)) ColorChangeMode = ColorChangeModeEnum.CrossOver;

        if (Input.GetKeyDown(KeyCode.R))
        {
            var value = DefaultValues[ColorChangeMode];
            SetActiveValue(value);
            Camera.ApplySettings();
            return;
        }

        float speed = Time.deltaTime * Speed;
        float delta = 0f;
        
        if (Input.GetKey(KeyCode.LeftShift)) speed *= ShiftMultiplier;
        if (Input.GetKey(KeyCode.RightShift)) speed = RShiftStep;

        if (InputExtensions.GetKeyModified(KeyCode.KeypadPlus)) delta = speed;
        if (InputExtensions.GetKeyModified(KeyCode.KeypadMinus)) delta = -speed;

        if (delta == 0) return;
        AddActiveValue(delta);
        Camera.ApplySettings();
    }
}
