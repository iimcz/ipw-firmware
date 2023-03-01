using emt_sdk.Events.Effect;
using Naki3D.Common.Protocol;
using UnityEngine;
using UnityEngine.Events;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CursorComponent : MonoBehaviour
{
    [SerializeField]
    private CameraRigSpawnerComponent _cameraRig;

    public float MinX = float.MaxValue;
    public float MaxX = float.MinValue;
    public float MinY = float.MaxValue;
    public float MaxY = float.MinValue;

    public TMPro.TMP_Text Text;

    public bool DebugOutput = false;
    public float ActivationTime = 2f;
    public UnityEvent<GameObject> OnActivate;

    public bool IsVisible => ScreenPos.y > 0.45f;
    public Vector3 ScreenPos;
    
    public GameObject HoveredObject { get; private set; }
    public float HoverTime { get; private set; }

    private SpriteRenderer _sprite;
    private Vector2 _viewportPos;
    private bool _activated;

    /// <summary>
    /// Whether the detection boundaries should be set to min/max detected values.
    /// E.g. when a user moves their hand further left than was the previous min value the entire mapping range rescales.
    /// </summary>
    [SerializeField]
    private bool _updateBoundaries;

    private void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (IsVisible)
        {
            // Try a 3D ray instead if we didn't hit anything
            if (!Cast2DRay()) Cast3DRay();
        }

        if (HoveredObject == null) _activated = false;
        if (!_activated && HoverTime > ActivationTime)
        {
            _activated = true;
            OnActivate.Invoke(HoveredObject);
        }

        if (DebugOutput)
        {
            var color = string.Empty;
            if (_activated) color = "<color=\"red\">";
            Text.text = $"{color}{_viewportPos}\n{HoveredObject} ({HoverTime}s)";
        }
    }

    private bool Cast2DRay()
    {
        var hit = Physics2D.OverlapPoint(transform.position);

        if (!hit)
        {
            HoveredObject = null;
            HoverTime = 0f;
            return false;
        }

        if (hit.gameObject == HoveredObject) HoverTime += Time.deltaTime;
        else HoverTime = 0f;
                
        HoveredObject = hit.gameObject;
        return true;
    }

    private bool Cast3DRay()
    {
        var ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * 50, Color.red);
        var didHit = Physics.Raycast(ray, out var hit, 500f);

        if (!didHit)
        {
            HoveredObject = null;
            HoverTime = 0f;
            return false;
        }

        if (hit.collider.gameObject == HoveredObject) HoverTime += Time.deltaTime;
        else HoverTime = 0f;
                
        HoveredObject = hit.collider.gameObject;
        return true;
    }

    private void UpdateBoundaries(Vector2 handPos)
    {
        if (handPos.x < MinX) MinX = handPos.x;
        if (handPos.y < MinY) MinY = handPos.y;

        if (handPos.x > MaxX) MaxX = handPos.x;
        if (handPos.y > MaxY) MaxY = handPos.y;
    }

    public void CursorPosition(Vector3 handPos)
    {
        if (_updateBoundaries) UpdateBoundaries(handPos);

        ScreenPos = handPos.Map(new Vector2(MinX, MinY), new Vector2(MaxX, MaxY), Vector2.zero, Vector2.one);

        var boundaries = _cameraRig.CameraRig.GetBoundaries(2.5f);
        var pos = _viewportPos.Map(Vector2.zero, Vector2.one, new Vector2(boundaries.xMin, boundaries.yMin), new Vector2(boundaries.xMin, boundaries.yMax));

        transform.localPosition = new Vector3(pos.x, pos.y, 2.5f);
        _sprite.enabled = IsVisible;
    }
}
