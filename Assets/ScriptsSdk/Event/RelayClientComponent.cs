using emt_sdk.Events.Relay;
using Naki3D.Common.Protocol;
using NLog;
using System.Threading.Tasks;
using UnityEngine.Events;

public class RelayClientComponent : MainThreadExecutorComponent
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public UnityEvent<SensorMessage> EventReceived;
    public bool LogEvents;

    private EventRelayClient _client;

    private void Start()
    {
        _client = new EventRelayClient();
        _client.OnEventReceived += OnEventReceived;

        Task.Run(() => _client.Connect());
    }

    private void OnDestroy()
    {
        if (_client.TokenSource != null) _client.TokenSource.Cancel();
        else Logger.Warn("Cannot cancel relay client because it never connected");
    }

    private void OnEventReceived(SensorMessage message)
    {
        if (LogEvents) print(message);
        ExecuteOnMainThread(() => EventReceived.Invoke(message));
    }

    public void BroadcastEvent(SensorMessage message)
    {
        _client.BroadcastEvent(message);
    }
}
