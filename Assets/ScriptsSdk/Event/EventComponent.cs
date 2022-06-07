using Assets.Extensions;
using emt_sdk.Events;
using Naki3D.Common.Protocol;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public UnityEvent<SensorMessage> EventReceived;

    void Start()
    {
        EventManager.Instance.OnEventReceived += OnEventReceived;
    }
    
    void OnDestroy()
    {
        EventManager.Instance.OnEventReceived -= OnEventReceived;
    }

    protected virtual void OnEventReceived(object sender, SensorMessage e)
    {
        Logger.DebugUnity(e.ToString());
        Logger.DebugUnity(e.DataCase.ToString());
        ExecuteOnMainThread(() => EventReceived.Invoke(e));
    }
}
