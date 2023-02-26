using emt_sdk.Events;
using Naki3D.Common.Protocol;
using System;
using System.Linq;
using UnityEngine;

public class LocalInputComponent : MonoBehaviour
{
    public ExhibitConnectionComponent ExhibitConnection;

    private UnityEngine.Vector2 _prevMousePos;
    private float _prevScroll;
    private EventManager _eventManager;

    private void Awake()
    {
        _prevMousePos = new UnityEngine.Vector2(Input.mousePosition.x, Input.mousePosition.y);
        _prevScroll = Input.mouseScrollDelta.y;
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
    }

    private string Hostname => ExhibitConnection == null ? string.Empty : ExhibitConnection.Hostname;

    // Update is called once per frame
    void Update()
    {
        var mousePos = new UnityEngine.Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (_prevMousePos != mousePos)
        {
            var absoluteMouseEvent = new SensorDataMessage
            {
                Path = $"{Hostname}_mouse/absolute",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds(),
                Vector2 = mousePos.ToNakiVector()
            };

            var relativeMouseEvent = new SensorDataMessage
            {
                Path = $"{Hostname}_mouse/relative",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds(),
                Vector2 = (mousePos - _prevMousePos).ToNakiVector()
            };

            _eventManager.BroadcastEvent(absoluteMouseEvent);
            _eventManager.BroadcastEvent(relativeMouseEvent);
            _prevMousePos = mousePos;
        }

        var scroll = Input.mouseScrollDelta.y;
        if (_prevScroll != scroll && scroll != 0)
        {
            var scrollEvent = new SensorDataMessage
            {
                Float = scroll,
                Path = $"{ExhibitConnection.Hostname}_mouse/scroll",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            _eventManager.BroadcastEvent(scrollEvent);
            _prevScroll = scroll;
        }

        foreach (var keyCode in Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>())
        {
            bool? state = null;

            if (Input.GetKeyDown(keyCode)) state = true;
            else if (Input.GetKeyUp(keyCode)) state = false;

            if (state == null) continue;
            var eventSuffix = state.Value ? "/key_down" : "/key_up";
            var keyboardEvent = new SensorDataMessage
            {
                Integer = (int)keyCode,
                Path = $"{Hostname}_keyboard{eventSuffix}",
                Timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            _eventManager.BroadcastEvent(keyboardEvent);
        }
    }
}
