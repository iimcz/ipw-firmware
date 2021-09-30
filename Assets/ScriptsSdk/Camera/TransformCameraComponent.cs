using Assets.ScriptsSdk.Extensions;
using emt_sdk.Settings;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TransformCameraComponent : MonoBehaviour
{
    [SceneObjectsOnly, Required]
    public Camera Camera;

    [SerializeField]
    public DisplaySetting Setting;

    public int TargetDisplay
    {
        get => Camera.targetDisplay;
        set
        {
            Camera.targetDisplay = value;
            Setting.DisplayId = value;
        }
    }

    public Rect GetCameraBoundries()
    {
        var topLeft = Camera.ViewportToWorldPoint(new Vector3(0, 0, Camera.nearClipPlane));
        var bottomRight = Camera.ViewportToWorldPoint(new Vector3(1, 1, Camera.nearClipPlane));
        var size = bottomRight - topLeft;

        return new Rect(topLeft.x, topLeft.y, size.x, size.y);
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

    public void ApplySettings()
    {
        ProjectorTransformationPass.ScreenMeshes[TargetDisplay] = ProjectorTransformationPass.CreateTransform(SettingsVertices);

        ProjectorTransformationPass.Brightness[TargetDisplay] = Setting.Color.Brightness;
        ProjectorTransformationPass.Contrast[TargetDisplay] = Setting.Color.Contrast;
        ProjectorTransformationPass.Saturation[TargetDisplay] = Setting.Color.Saturation;

        ProjectorTransformationPass.FlipCurve[TargetDisplay] = TargetDisplay == 0;

        Camera.targetDisplay = Setting.DisplayId;
    }

    private Vector3[] SettingsVertices => new[] { 
        Setting.Skew.BottomLeft.ToUnityVector(),
        Setting.Skew.TopLeft.ToUnityVector(),
        Setting.Skew.TopRight.ToUnityVector(),
        Setting.Skew.BottomRight.ToUnityVector(),
    }.Select(v => new Vector3(v.x, v.y)).ToArray();
}
