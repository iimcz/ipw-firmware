using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UserDotComponent : MonoBehaviour
{
    [Min(0.01f)]
    public float InactivityDelay = 5.0f;

    [Min(1f)]
    public float GestureAnimationMagnitude = 50.0f;

    [SerializeField]
    private Image _image;

    private RectTransform _rectTransform;
    private float _lastActivity = 0.0f;
    private bool _notifying = false;

    private float _target = 0.0f;
    private float _position = 0.0f;
    
    private float _targetConfidence = 1.0f;
    private float _confidence = 1.0f;

    private Vector2 _targetGesturePosition = Vector2.zero;

    [SerializeField]
    private AnimationCurve _confidenceScaleCurve;

    void Start()
    {
        _rectTransform = transform as RectTransform;
    }

    public void UpdatePosition(float position)
    {
        _lastActivity = 0.0f;

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            SetPosition(position);
        }

        _target = position;
    }

    public void UpdateConfidence(float confidence)
    {
        _targetConfidence = _confidenceScaleCurve.Evaluate(confidence);
    }

    public void AnimateGesture(string type)
    {
        // Reset in case we're already animating
        _rectTransform.anchoredPosition = Vector2.zero;

        switch (type)
        {
            case "swipe_left":
                _targetGesturePosition = new Vector2(-GestureAnimationMagnitude, 0);
                break;
            case "swipe_up":
                _targetGesturePosition = new Vector2(0, GestureAnimationMagnitude);
                break;
            case "swipe_right":
                _targetGesturePosition = new Vector2(GestureAnimationMagnitude, 0);
                break;
            case "swipe_down":
                _targetGesturePosition = new Vector2(0, -GestureAnimationMagnitude);
                break;
        }
    }

    public IEnumerator Notify()
    {
        if (_notifying) yield break;

        _notifying = true;

        for (int i = 0; i < 50; i++)
        {
            var scale = 1 + Mathf.Abs(Mathf.Sin(i / 2f));
            _rectTransform.localScale = Vector3.one * scale;
            yield return new WaitForSeconds(0.05f);
        }

        _notifying = false;
        _confidence = _targetConfidence;
        _rectTransform.localScale = Vector3.one * _targetConfidence;
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

    void Update()
    {
        _lastActivity += Time.deltaTime;

        if (_lastActivity > InactivityDelay) gameObject.SetActive(false);

        // Intentional "wrong" lerp use for smoothing
        SetPosition(Mathf.Lerp(_position, _target, 0.5f));

        if (!_notifying && Mathf.Abs(_confidence - _targetConfidence) > 0.05f)
        {
            _confidence = Mathf.Lerp(_confidence, _targetConfidence, 0.5f);
            _rectTransform.localScale = Vector3.one * _confidence;
        }

        // Gesture anim
        _rectTransform.anchoredPosition = Vector2.Lerp(_rectTransform.anchoredPosition, _targetGesturePosition, 0.5f);
        var distance = (_rectTransform.anchoredPosition - _targetGesturePosition).magnitude;
        if (distance <= 0.05f) _targetGesturePosition = Vector2.zero;
    }

    private void SetPosition(float position)
    {
        var vectorPos = new Vector2(position, 0.5f);
        _rectTransform.anchorMax = vectorPos;
        _rectTransform.anchorMin = vectorPos;

        _position = position;
    }
}
