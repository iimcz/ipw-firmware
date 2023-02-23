using Assets.Extensions;
using emt_sdk.Events;
using Naki3D.Common.Protocol;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public UnityEvent<SensorMessage> EventReceived;
    public bool LogEvents;

    void Start()
    {
        // EventManager.Instance.OnEventReceived += OnEventReceived;
    }
    
    void OnDestroy()
    {
        // EventManager.Instance.OnEventReceived -= OnEventReceived;
    }

    protected virtual void OnEventReceived(SensorMessage e)
    {
        if (LogEvents) Logger.DebugUnity(e.ToString());
        
        ExecuteOnMainThread(() => EventReceived.Invoke(e));
    }
}
