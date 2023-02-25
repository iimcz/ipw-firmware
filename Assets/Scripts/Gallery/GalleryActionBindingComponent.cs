using Assets.Extensions;
using emt_sdk.Events;
using emt_sdk.Events.Effect;
using NLog;
using UnityEngine;

public class GalleryActionBindingComponent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    [SerializeField]
    private GalleryPoolComponent _pool;

    [SerializeField]
    private GalleryCursorComponent _cursor;

    private EventManager _eventManager;

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
        var effect = e.Name.ToLowerInvariant();
        var hasValue = e.DataType == Naki3D.Common.Protocol.DataType.Integer;
        var value = hasValue ? e.Integer : 1;

        switch (effect)
        {
            case "left":
                ExecuteOnMainThread(() =>
                {
                    for (int i = 0; i < value; i++) _pool.Layout.Previous();
                });
            break;
            case "right":
                ExecuteOnMainThread(() =>
                {
                    for (int i = 0; i < value; i++) _pool.Layout.Next();
                });
                break;
            case "scrollTo":
                ExecuteOnMainThread(() =>
                {
                    _pool.Layout.ScrollTo(value);
                });
                break;
            case "zoom":
                ExecuteOnMainThread(() =>
                {
                    _pool.Layout.ScrollTo(value);
                    var gameObject = _pool.Layout.GetImage(value);

                    _cursor.Activate(gameObject);
                });
                break;
            case "unzoom":
                ExecuteOnMainThread(() =>
                {
                    _cursor.Deactivate();
                });
                break;
            default:
                return;
        }

        Logger.InfoUnity($"[Gallery Action] {effect}: {(hasValue ? value.ToString() : "void")}");
    }
}
