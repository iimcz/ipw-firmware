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

    public int ActiveVertex = 0;

    public float Speed = 0.5f;
    public float ShiftMultiplier = 0.25f;

    private Vector2[] _vertices = new Vector2[4];

    void Start()
    {
        _vertices[0] = Camera.Setting.Skew.BottomLeft.ToUnityVector();
        _vertices[1] = Camera.Setting.Skew.TopLeft.ToUnityVector();
        _vertices[2] = Camera.Setting.Skew.TopRight.ToUnityVector();
        _vertices[3] = Camera.Setting.Skew.BottomRight.ToUnityVector();

        Camera.SetTransform(_vertices);
    }

    void OnEnable()
    {
        Crosshair.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        Crosshair.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ActiveVertex++;
            ActiveVertex %= 4;

            var rootRect = ((RectTransform)Crosshair.root).rect;
            Crosshair.anchoredPosition = ActiveVertex switch
            {
                0 => new Vector2(0, -rootRect.height),
                1 => new Vector2(0, 0),
                2 => new Vector2(rootRect.width, 0),
                3 => new Vector2(rootRect.width, -rootRect.height),
                _ => throw new NotImplementedException(),
            };
        }

        float speed = Time.deltaTime * Speed;
        Vector2 offset = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftShift)) speed *= 0.25f;

        if (Input.GetKey(KeyCode.UpArrow)) offset += new Vector2(0, speed);
        if (Input.GetKey(KeyCode.DownArrow)) offset += new Vector2(0, -speed);
        if (Input.GetKey(KeyCode.LeftArrow)) offset += new Vector2(-speed, 0);
        if (Input.GetKey(KeyCode.RightArrow)) offset += new Vector2(speed, 0);

        if (offset == Vector2.zero) return;

        _vertices[ActiveVertex] += offset;

        for (int i = 0; i < _vertices.Length; i++)
        {
            float x = Mathf.Clamp(_vertices[i].x, -1f, 1f);
            float y = Mathf.Clamp(_vertices[i].y, -1f, 1f);

            _vertices[i] = new Vector2(x, y);
        }

        Camera.SetTransform(_vertices);
    }
}
