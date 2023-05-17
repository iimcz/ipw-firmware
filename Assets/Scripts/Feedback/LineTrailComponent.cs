using UnityEngine;
using UnityEngine.UIElements;

public class LineTrailComponent : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    public int FrameHistorySize = 60;

    private Vector3[] _positions;

    void Start()
    {
        _lineRenderer.positionCount = FrameHistorySize;

        _positions = new Vector3[FrameHistorySize];
        for (int i = 0; i < FrameHistorySize; i++) _positions[i] = transform.position;
    }

    void Update()
    {
        for (int i = FrameHistorySize - 1; i >= 1; i--) _positions[i] = _positions[i - 1];
        _positions[0] = transform.position;

        _lineRenderer.SetPositions(_positions);
    }
}
