using System;
using Assets.Extensions;
using Assets.ScriptsSdk.Extensions;
using emt_sdk.Events;
using emt_sdk.Events.Effect;
using Naki3D.Common.Protocol;
using UnityEngine;
using UnityEngine.Events;

public class ActionEvent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public string Effect;

    [SerializeField]
    private DataType EffectType;
    private EventManager _eventManager;
    public UnityEvent<float> FloatEffectCalled;
    public UnityEvent<int> IntEffectCalled;
    public UnityEvent<bool> BoolEffectCalled;
    public UnityEvent<string> StringEffectCalled;
    public UnityEvent VoidEffectCalled;

    public UnityEvent<Vector2> Vector2EffectCalled;
    public UnityEvent<Vector3> Vector3EffectCalled;

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _eventManager.OnEffectCalled += OnEventReceived;
    }

    void OnDestroy()
    {
        _eventManager.OnEffectCalled -= OnEventReceived;
    }

    private void OnEventReceived(EffectCall e)
    {
        if (e.DataType != EffectType || !string.Equals(e.Name, Effect, StringComparison.CurrentCultureIgnoreCase)) return;
        //if (!string.Equals(e.Name, Effect, StringComparison.CurrentCultureIgnoreCase)) return;

        Logger.InfoUnity($"[Action] {Effect}: {e.DataType.ToString()}");

        Action actionCall = e.DataType switch
        {
            DataType.Void => () => VoidEffectCalled.Invoke(),
            DataType.Bool => () => BoolEffectCalled.Invoke(e.Bool),
            DataType.Integer => () => IntEffectCalled.Invoke(e.Integer),
            DataType.Float => () => FloatEffectCalled.Invoke(e.Float),
            DataType.String => () => StringEffectCalled.Invoke(e.String),

            DataType.Vector2 => () => Vector2EffectCalled.Invoke(e.Vector2.ToUnityVector()),
            DataType.Vector3 => () => Vector3EffectCalled.Invoke(e.Vector3.ToUnityVector()),
            _ => () => Logger.Log(NLog.LogLevel.Warn, $"Unknown effec type: {e.DataType.ToString()}")
        };

        ExecuteOnMainThread(actionCall);
    }
}
