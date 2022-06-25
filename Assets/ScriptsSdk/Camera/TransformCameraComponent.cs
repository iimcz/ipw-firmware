using Assets.ScriptsSdk.Extensions;
using emt_sdk.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransformCameraComponent : MonoBehaviour
{
    public Camera Camera;

    public bool AlignCorners;
    
    public List<DisplaySetting> Settings;

    public int SettingIndex
    {
        get => _settingIndex;
        set
        {
            _settingIndex = value;
            ApplySettings();
        }
    }

    public int TargetDisplay
    {
        get => Camera.targetDisplay;
        set
        {
            _targetDisplay = value;
            Camera.targetDisplay = value;
            Setting.DisplayId = value;
            
            ApplySettings();
        }
    }

    public DisplaySetting Setting => Settings[SettingIndex];

    [SerializeField]
    [Range(0, 7)]
    private int _settingIndex = 0;

    [SerializeField]
    [Range(0, 7)]
    private int _targetDisplay;

    public Rect GetCameraBoundries()
    {
        return GetCameraBoundries(Camera.nearClipPlane);
    }
    
    public Rect GetCameraBoundries(float distance)
    {
        var bottomLeft = Camera.ViewportToWorldPoint(new Vector3(0, 0, distance));
        var topRight = Camera.ViewportToWorldPoint(new Vector3(1, 1, distance));

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    public void SetTransform(Vector2[] vertices)
    {
        if (vertices == null) throw new ArgumentNullException(nameof(vertices));
        if (vertices.Length != 4) throw new NotSupportedException("Transform must be 2 quads (8 vertices)");

        Setting.Skew.BottomLeft = vertices[0].ToNakiVector();
        Setting.Skew.TopLeft = vertices[1].ToNakiVector();
        Setting.Skew.TopRight = vertices[2].ToNakiVector();
        Setting.Skew.BottomRight = vertices[3].ToNakiVector();

        ApplySettings();
    }

    /// <summary>
    /// Applies loaded settings into the transformation render pass
    /// </summary>
    public void ApplySettings()
    {
        _targetDisplay = Setting.DisplayId;
        Camera.targetDisplay = Setting.DisplayId;
        Camera.aspect = 1;
        
        ProjectorTransformationPass.ScreenData[TargetDisplay].ScreenMesh = MeshUtils.CreateTransform(SettingsVertices, AlignCorners);

        ProjectorTransformationPass.Brightness[TargetDisplay] = Setting.Color.Brightness.ToUnityColor();
        ProjectorTransformationPass.Contrast[TargetDisplay] = Setting.Color.Contrast;
        ProjectorTransformationPass.Saturation[TargetDisplay] = Setting.Color.Saturation;

        ProjectorTransformationPass.FlipCurve[TargetDisplay] = TargetDisplay == 0;
        ProjectorTransformationPass.CrossOver[TargetDisplay] = Setting.CrossOver;
    }

    private Vector3[] SettingsVertices => new[] { 
        Setting.Skew.BottomLeft.ToUnityVector(),
        Setting.Skew.TopLeft.ToUnityVector(),
        Setting.Skew.TopRight.ToUnityVector(),
        Setting.Skew.BottomRight.ToUnityVector(),
    }.Select(v => new Vector3(v.x, v.y)).ToArray();
}
