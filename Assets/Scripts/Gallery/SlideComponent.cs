using UnityEngine;

public class SlideComponent : MonoBehaviour
{
    public Vector3 Target;
    public AnimationCurve Animation;

    public float Length;

    [SerializeField]
    private float _progress;
    private Vector3 _start;

    public void SetTarget(Vector3 t)
    {
        Target = t;

        _start = transform.localPosition;
        _progress = 0;
    }

    public void ResetPosition()
    {
        _start = transform.localPosition;
        _progress = Length;
    }

    void Start()
    {
        ResetPosition();
    }

    void Update()
    {
        if (_progress >= Length) return;

        _progress += Time.deltaTime;
        if (_progress >= Length)
        {
            _progress = Length;
            transform.localPosition = Target;
            _start = Target;
        }
        else
        {
            var animationProgress = Animation.Evaluate(_progress / Length);
            transform.localPosition = Vector3.Lerp(_start, Target, animationProgress);
        }
    }
}
