using emt_sdk.Events;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using Naki3D.Common.Protocol;
using UnityEngine;

[RequireComponent(typeof(NodeRotatorComponent))]
public class DebugInputComponent : MonoBehaviour
{
    private NodeRotatorComponent _rotator;
    private EventManager _eventManager;

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _rotator = GetComponent<NodeRotatorComponent>();

        _eventManager.ConnectSensor();
    }

    public void SwipeLeftGesture(SensorDataMessage message)
    {
        _rotator.TurnLeft();
    }

    public void SwipeRightGesture(SensorDataMessage message)
    {
        _rotator.TurnRight();
    }

    public void SwipeUpGesture(SensorDataMessage message)
    {
        _rotator.Activate();
    }

    public void KbUpdate(SensorDataMessage message)
    {
        var keyCode = (KeyCode) message.Integer;

        switch (keyCode)
        {
            case KeyCode.LeftArrow:
                _rotator.TurnLeft();
                break;
            case KeyCode.RightArrow:
                _rotator.TurnRight();
                break;
            case KeyCode.Space:
                _rotator.Activate();
                break;
        }
    }
}
