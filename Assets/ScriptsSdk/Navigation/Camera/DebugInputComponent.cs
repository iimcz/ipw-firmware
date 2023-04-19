using System.Threading.Tasks;
using emt_sdk.Events;
using emt_sdk.Events.Local;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using Naki3D.Common.Protocol;
using UnityEngine;

[RequireComponent(typeof(NodeRotatorComponent))]
public class DebugInputComponent : MonoBehaviour
{
    private NodeRotatorComponent _rotator;
    private EventManager _eventManager;
    public bool Standalone = false;


    void Start()
    {
        if (Standalone)
        {
            var sensorManager = GlobalServices.Instance.GetRequiredService<ISensorManager>();
            Task.Run(() => sensorManager.Start());
        }

        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _rotator = GetComponent<NodeRotatorComponent>();

        _eventManager.ConnectSensor();
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

    public void GestureUpdate(SensorDataMessage message)
    {
        if (message.Path.StartsWith("/")) return; // Ignore remote messages
        if (message.Path.EndsWith("swipe_up"))
        {
            _rotator.Activate();
        }
        else if (message.Path.EndsWith("swipe_left"))
        {
            _rotator.TurnLeft();
        }
        else if (message.Path.EndsWith("swipe_right"))
        {
            _rotator.TurnRight();
        }
    }
}
