using UnityEngine;

public class MotionDisplayComponent : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    public MotionVectorComponent VectorComponent;

    void Start()
    {
    }

    void Update()
    {
        _lineRenderer.SetPosition(1, VectorComponent.TargetDirection * 50f);
    }
}
