using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LensShiftComponent : MonoBehaviour
{
    public float Speed = 0.001f;
    public float ShiftMultiplier = 0.1f;
    public float RShiftStep = 0.001f;

    [SerializeField] private DualCameraComponent _camera;
    
    void Update()
    {
        var delta = 0f;
        if (InputExtensions.GetKeyModified(KeyCode.KeypadPlus)) delta = Speed;
        else if (InputExtensions.GetKeyModified(KeyCode.KeypadMinus)) delta = -Speed;

        if (Input.GetKey(KeyCode.LeftShift)) delta *= ShiftMultiplier;
        if (delta != 0f && Input.GetKey(KeyCode.RightShift)) delta = Mathf.Sign(delta) * RShiftStep;

        if (delta == 0f) return;
        _camera.Setting.LensShift += delta;
        _camera.ApplySettings();
    }
}
