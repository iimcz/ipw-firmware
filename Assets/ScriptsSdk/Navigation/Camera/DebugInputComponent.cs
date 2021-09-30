using emt_sdk.Events;
using Naki3D.Common.Protocol;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(NodeRotatorComponent))]
public class DebugInputComponent : MonoBehaviour
{
    private NodeRotatorComponent _rotaror;

    void Start()
    {
        _rotaror = GetComponent<NodeRotatorComponent>();

        Task.Run(() => EventManager.Instance.Start(null));
    }

    public void GestureUpdate(SensorMessage message)
    {
        switch (message.Gesture.Type)
        {
            case GestureType.GestureSwipeLeft:
                _rotaror.TurnLeft();
                break;
            case GestureType.GestureSwipeRight:
                _rotaror.TurnRight();
                break;
            case GestureType.GestureSwipeUp:
                _rotaror.Activate();
                break;
        }
    }

    public void KbUpdate(SensorMessage message)
    {
        if (message.KeyboardUpdate.Type == KeyActionType.KeyUp) return;
        var keyCode = (KeyCode) message.KeyboardUpdate.Keycode;

        switch (keyCode)
        {
            case KeyCode.LeftArrow:
                _rotaror.TurnLeft();
                break;
            case KeyCode.RightArrow:
                _rotaror.TurnRight();
                break;
            case KeyCode.Space:
                _rotaror.Activate();
                break;
        }
    }
}
