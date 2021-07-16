using emt_sdk.Events;
using Naki3D.Common.Protocol;
using System;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Events;

public class EventComponent : MonoBehaviour
{
    public UnityEvent<SensorMessage> EventReceived;

    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    void Start()
    {
        EventManager.Instance.OnEventReceived += OnEventReceived;
    }

    void Update()
    {
        while (_mainThreadActions.TryDequeue(out var action)) action();
    }

    protected virtual void OnEventReceived(object sender, SensorMessage e)
    {
        _mainThreadActions.Enqueue(() => EventReceived.Invoke(e));
    }
}
