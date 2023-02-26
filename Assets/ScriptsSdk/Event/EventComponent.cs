using Assets.Extensions;
using emt_sdk.Events;
using Naki3D.Common.Protocol;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public UnityEvent<SensorDataMessage> EventReceived;
    public bool LogEvents;
    private EventManager _eventManager;

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _eventManager.OnEventReceived += OnEventReceived;
    }
    
    void OnDestroy()
    {
        _eventManager.OnEventReceived -= OnEventReceived;
    }

    protected virtual void OnEventReceived(SensorDataMessage e)
    {
        if (LogEvents) Logger.DebugUnity(e.ToString());
        
        ExecuteOnMainThread(() => EventReceived.Invoke(e));
    }
}
