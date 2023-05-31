using Assets.ScriptsSdk.Extensions;
using Naki3D.Common.Protocol;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class HandTrackerComponent : MonoBehaviour
{
    [SerializeField]
    private RectTransform _lHand;

    [SerializeField]
    private RectTransform _rHand;

    [SerializeField]
    private Image _rHandGesture;

    [SerializeField]
    private Image _lHandGesture;

    [Min(0.01f)]
    public float GestureInactivityDelay = 5.0f;
    private float _lastGestureActivity = 0.0f;


    void Update()
    {
        _lastGestureActivity += Time.deltaTime;
        if (_lastGestureActivity > GestureInactivityDelay)
        {
            _rHandGesture.enabled = false;
            _lHandGesture.enabled = false;
        }

#if UNITY_EDITOR
        HandMove(new SensorDataMessage
        {
            Path = "mediapipe/handtracking/hand/right/center_position",
            Vector3 = new Vector3Data
            {
                X = Input.mousePosition.x / Screen.width,
                Y = Input.mousePosition.y / Screen.height,
                Z = 45
            }
        });

        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowGesture(_rHand, "swipe_left");
        else if (Input.GetKeyDown(KeyCode.UpArrow)) ShowGesture(_rHand, "swipe_up");
        else if (Input.GetKeyDown(KeyCode.RightArrow)) ShowGesture(_rHand, "swipe_right");
        else if (Input.GetKeyDown(KeyCode.DownArrow)) ShowGesture(_rHand, "swipe_down");
        else return;

        _lastGestureActivity = 0f;
        _rHandGesture.enabled = true;
#endif
    }


    public void HandMove(SensorDataMessage message)
    {
        var parts = message.Path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
        var hand = parts[^2];

        RectTransform handRect = null;
        if (hand == "left") handRect = _lHand;
        else if (hand == "right") handRect = _rHand;
        else 
        {
            Debug.LogWarning($"Unknown hand '{hand}'");
            return;
        }

        if (parts.Last() == "center_position")
        {
            var position = message.Vector3.ToUnityVector();
            position = Vector3.one - (position * 2);
            position.Scale(new Vector3(-750, 750, 0));
            handRect.anchoredPosition = position;
        }
    }

    public void HandGesture(SensorDataMessage message)
    {
        var parts = message.Path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);
        var hand = parts[^3];

        RectTransform handRect = null;
        if (hand == "left") handRect = _lHand;
        else if (hand == "right") handRect = _rHand;
        else 
        {
            Debug.LogWarning($"Unknown hand '{hand}'");
            return;
        }

        if (parts[^2] == "gestures")
        {
            _lastGestureActivity = 0f;
            if (hand == "left") _lHandGesture.enabled = true;
            else if (hand == "right") _rHandGesture.enabled = true;

            ShowGesture(handRect, parts.Last());
        }
    }

    private void ShowGesture(RectTransform handRect, string direction)
    {
        switch (direction)
        {
            case "swipe_left":
                handRect.localEulerAngles = new Vector3(0, 0, 180);
                break;
            case "swipe_right":
                handRect.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case "swipe_up":
                handRect.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case "swipe_down":
                handRect.localEulerAngles = new Vector3(0, 0, 270);
                break;
        }
    }
}
