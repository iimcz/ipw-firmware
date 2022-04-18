using Assets.ScriptsSdk.Extensions;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class CornerMoveComponent : MonoBehaviour
{
    [Required, SceneObjectsOnly]
    public TransformCameraComponent Camera;

    [Required, SceneObjectsOnly]
    public RectTransform Crosshair;

    public int ActiveVertex;
    public bool AlignCorners;

    public float Speed = 0.5f;
    public float ShiftMultiplier = 0.25f;
    public float RShiftStep = 0.001f;

    private readonly Vector2[] _vertices = new Vector2[4];

    private void Start()
    {
        _vertices[0] = Camera.Setting.Skew.BottomLeft.ToUnityVector();
        _vertices[1] = Camera.Setting.Skew.TopLeft.ToUnityVector();
        _vertices[2] = Camera.Setting.Skew.TopRight.ToUnityVector();
        _vertices[3] = Camera.Setting.Skew.BottomRight.ToUnityVector();

        Camera.SetTransform(_vertices);
    }

    private void OnEnable()
    {
        Crosshair.gameObject.SetActive(true);
        ProjectorTransformationPass.EnableBlending = false;
        UpdateCrosshair();
    }

    private void OnDisable()
    {
        ProjectorTransformationPass.EnableBlending = true;
        Crosshair.gameObject.SetActive(false);
    }

    private void UpdateCrosshair()
    {
        var rootRect = ((RectTransform)Crosshair.root).rect;

        // TODO: maybe use loaded settings instead of set renderpass params
        if (ProjectorTransformationPass.Vertical)
        {
            Crosshair.anchoredPosition = ActiveVertex switch
            {
                0 => new Vector2(rootRect.width, -rootRect.height),
                1 => new Vector2(0, -rootRect.height),
                2 => new Vector2(0, 0),
                3 => new Vector2(rootRect.width, 0),
                _ => throw new NotImplementedException(),
            };
        }
        else
        {
            Crosshair.anchoredPosition = ActiveVertex switch
            {
                0 => new Vector2(0, -rootRect.height),
                1 => new Vector2(0, 0),
                2 => new Vector2(rootRect.width, 0),
                3 => new Vector2(rootRect.width, -rootRect.height),
                _ => throw new NotImplementedException(),
            };
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ActiveVertex++;
            ActiveVertex %= 4;
            
            UpdateCrosshair();
        }

        var speed = Time.deltaTime * Speed;
        var offset = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftShift)) speed *= ShiftMultiplier;
        if (Input.GetKey(KeyCode.RightShift)) speed = RShiftStep;

        // TODO: maybe use loaded settings instead of set renderpass params
        if (ProjectorTransformationPass.Vertical)
        {
            if (InputExtensions.GetKeyModified(KeyCode.UpArrow)) offset += new Vector2(speed, 0);
            if (InputExtensions.GetKeyModified(KeyCode.DownArrow)) offset += new Vector2(-speed, 0);
            if (InputExtensions.GetKeyModified(KeyCode.LeftArrow)) offset += new Vector2(0, speed);
            if (InputExtensions.GetKeyModified(KeyCode.RightArrow)) offset += new Vector2(0, -speed);
        }
        else
        {
            if (InputExtensions.GetKeyModified(KeyCode.UpArrow)) offset += new Vector2(0, speed);
            if (InputExtensions.GetKeyModified(KeyCode.DownArrow)) offset += new Vector2(0, -speed);
            if (InputExtensions.GetKeyModified(KeyCode.LeftArrow)) offset += new Vector2(-speed, 0);
            if (InputExtensions.GetKeyModified(KeyCode.RightArrow)) offset += new Vector2(speed, 0);   
        }
        

        if (offset == Vector2.zero) return;

        _vertices[ActiveVertex] += offset;

        for (var i = 0; i < _vertices.Length; i++)
        {
            var x = Mathf.Clamp(_vertices[i].x, -1f, 1f);
            var y = Mathf.Clamp(_vertices[i].y, -1f, 1f);

            _vertices[i] = new Vector2(x, y);
        }

        Camera.SetTransform(_vertices);
    }
}
