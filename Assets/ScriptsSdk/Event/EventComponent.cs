using emt_sdk.Events;
using Naki3D.Common.Protocol;
using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : MainThreadExecutorComponent
{
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
        Debug.Log(e);
        Debug.Log(e.DataCase);
        ExecuteOnMainThread(() => EventReceived.Invoke(e));
    }
}
