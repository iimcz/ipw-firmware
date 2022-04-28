using System;
using UnityEngine;
using emt_sdk.Settings;
using TMPro;

public class LensShiftComponent : MonoBehaviour
{
    public float Speed = 0.001f;
    public float ShiftMultiplier = 0.1f;
    public float RShiftStep = 0.001f;

    private float _baseLensShift = 0.5f;

    [SerializeField] private DualCameraComponent _camera;
    [SerializeField] private TextMeshProUGUI _modeText;
    
    void OnEnable()
    {
        // HACK: expect two cameras with same resolution
        switch (_camera.Orientation)
        {
            case IPWSetting.IPWOrientation.Single:
                break;
            case IPWSetting.IPWOrientation.Vertical:
            case IPWSetting.IPWOrientation.Horizontal:
                RShiftStep = 1.0f / _camera.TopCamera.Camera.pixelWidth;
                Speed = 5.0f / _camera.TopCamera.Camera.pixelWidth;
                break;
        }
    }

    void Update()
    {
        var delta = 0f;
        if (InputExtensions.GetKeyModified(KeyCode.KeypadPlus)) delta = Speed;
        else if (InputExtensions.GetKeyModified(KeyCode.KeypadMinus)) delta = -Speed;

        if (InputExtensions.GetKeyModified(KeyCode.Equals)) delta = Speed;
        else if (InputExtensions.GetKeyModified(KeyCode.Minus)) delta = -Speed;

        if (InputExtensions.GetKeyModified(KeyCode.Period)) delta = Speed;
        else if (InputExtensions.GetKeyModified(KeyCode.Comma)) delta = -Speed;


        if (Input.GetKey(KeyCode.LeftShift)) delta *= ShiftMultiplier;
        if (delta != 0f && Input.GetKey(KeyCode.RightShift)) delta = Mathf.Sign(delta) * RShiftStep;

        if (delta == 0f) return;
        _camera.Setting.LensShift += delta;
        _camera.Setting.LensShift = (int) (_camera.Setting.LensShift / RShiftStep) * RShiftStep;

        foreach (var display in _camera.Setting.Displays)
        {
            display.CrossOver = Math.Abs(_camera.Setting.LensShift - _baseLensShift) * 2.0f;
        }
        _camera.ApplySettings();

        _modeText.text = $"Kalibrace prostoru (3 / 6)\n\n" +
                         $"Aktuální posun: {(_camera.Setting.LensShift - _baseLensShift) * _camera.TopCamera.Camera.pixelWidth} pixelů\n\n" +
                         $"+ \t Zvýšení vzdálenosti\n" +
                         $"- \t Snížení vzdálenosti";
    }
}
