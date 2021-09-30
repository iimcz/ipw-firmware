using emt_sdk.Events;
using Naki3D.Common.Protocol;
using System;
using System.Linq;
using UnityEngine;

public class LocalInputComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;

    private EventManager _manager = EventManager.Instance;

    private UnityEngine.Vector2 _prevMousePos;
    private float _prevScroll;

    private void Awake()
    {
        _prevMousePos = new UnityEngine.Vector2(Input.mousePosition.x, Input.mousePosition.y);
        _prevScroll = Input.mouseScrollDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePos = new UnityEngine.Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (_prevMousePos != mousePos)
        {
            var mouseEvent = new SensorMessage
            {
                MouseMove = new MouseMoveData
                {
                    Absolute = mousePos.ToNakiVector(),
                    Relative = (mousePos - _prevMousePos).ToNakiVector()
                },
                 SensorId = $"{ExhibitConnection.Hostname}_mouse",
                 Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            _manager.BroadcastEvent(mouseEvent);
            _prevMousePos = mousePos;
        }

        var scroll = Input.mouseScrollDelta.y;
        if (_prevScroll != scroll && scroll != 0)
        {
            var scrollEvent = new SensorMessage
            {
                MouseScroll = new MouseScrollData
                {
                    Type = (scroll > 0) ? MouseScrollType.ScrollUp : MouseScrollType.ScrollDown
                },
                SensorId = $"{ExhibitConnection.Hostname}_mouse",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            _manager.BroadcastEvent(scrollEvent);
            _prevScroll = scroll;
        }

        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            bool? state = null;

            if (Input.GetKeyDown(keyCode)) state = true;
            else if (Input.GetKeyUp(keyCode)) state = false;

            if (state == null) continue;
            var keyboardEvent = new SensorMessage
            {
                KeyboardUpdate = new KeyboardUpdateData
                {
                    Keycode = (int)keyCode,
                    Type = state.Value ? KeyActionType.KeyDown : KeyActionType.KeyUp
                },
                SensorId = $"{ExhibitConnection.Hostname}_keyboard",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            _manager.BroadcastEvent(keyboardEvent);
        }
    }
}
