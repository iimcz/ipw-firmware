using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadExecutorComponent : MonoBehaviour
{
    private readonly ConcurrentQueue<Action> _mainThreadActions = new ConcurrentQueue<Action>();

    protected virtual void Update()
    {
        while (_mainThreadActions.TryDequeue(out var action)) action();
    }

    protected void ExecuteOnMainThread(Action action)
    {
        _mainThreadActions.Enqueue(action);
    }
}