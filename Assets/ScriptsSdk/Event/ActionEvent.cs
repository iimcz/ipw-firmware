using System;
using Assets.Extensions;
using emt_sdk.Events;
using UnityEngine;
using UnityEngine.Events;

public class ActionEvent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public string Effect;

    [SerializeField]
    private bool HasValue;
    public UnityEvent<double> ValueEffectCalled;
    public UnityEvent VoidEffectCalled;

    void Start()
    {
        EventManager.Instance.OnEffectCalled += OnEventReceived;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnEffectCalled -= OnEventReceived;
    }

    private void OnEventReceived(object sender, EffectCall e)
    {
        if (!string.Equals(e.Name, Effect, StringComparison.CurrentCultureIgnoreCase)) return;

        Logger.InfoUnity($"[Action] {Effect}: {e.Value?.ToString() ?? "void"}");
        
        if (HasValue && e.Value.HasValue) ExecuteOnMainThread(() => ValueEffectCalled.Invoke(e.Value.Value));
        else ExecuteOnMainThread(() => VoidEffectCalled.Invoke());
    }
}
