using UnityEngine;

public class MotionVectorComponent : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    public int FrameHistorySize = 20;

    private Vector3[] _positions;
    private Vector3 _prevPosition;

    public Vector3 TargetDirection;

    void Start()
    {
        _positions = new Vector3[FrameHistorySize];
        _prevPosition = transform.localPosition;
    }

    void Update()
    {
        // Distance from last frame
        var delta = transform.localPosition - _prevPosition;
        _prevPosition = transform.localPosition;

        for (int i = FrameHistorySize - 1; i >= 1; i--) _positions[i] = _positions[i - 1];
        _positions[0] = delta;

        var weight = 1f;
        var total = Vector3.zero;
        var totalWeight = 0f;

        for (int i = 0; i < FrameHistorySize; i++)
        {
            total += _positions[i] * weight;
            totalWeight += weight;

            weight -= 1f / FrameHistorySize;
        }
        total /= totalWeight;

        _lineRenderer.SetPosition(1, total * 10f);
        TargetDirection = total;
    }
}
