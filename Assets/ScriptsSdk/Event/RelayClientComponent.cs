using emt_sdk.Events;
using emt_sdk.Events.Relay;
using Naki3D.Common.Protocol;
using NLog;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class RelayClientComponent : MainThreadExecutorComponent
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public UnityEvent<SensorDataMessage> EventReceived;
    public bool LogEvents;

    private EventRelayClient _client;
    private EventManager _eventManager;

    private void Start()
    {
        _client = LevelScopeServices.Instance.GetRequiredService<EventRelayClient>();
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _client.OnEventReceived += OnEventReceived;

        Task.Run(() => _client.Connect());
    }

    private void OnDestroy()
    {
        _client.OnEventReceived -= OnEventReceived;
        if (_client.TokenSource != null) _client.TokenSource.Cancel();
        else Logger.Warn("Cannot cancel relay client because it never connected");
    }

    private void OnEventReceived(SensorDataMessage message)
    {
        if (LogEvents && ShouldLogType(message.DataCase)) print(message);
        _eventManager.HandleMessage(message);
        ExecuteOnMainThread(() => EventReceived.Invoke(message));
    }

    private bool ShouldLogType(SensorDataMessage.DataOneofCase dataCase)
    {
        switch(dataCase)
        {
            case SensorDataMessage.DataOneofCase.Vector3:
            case SensorDataMessage.DataOneofCase.Vector2:
                return false;
            default:
                return true;
        }
    }

    public void BroadcastEvent(SensorDataMessage message)
    {
        _client.BroadcastEvent(message);
    }
}
