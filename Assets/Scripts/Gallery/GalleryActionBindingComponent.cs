using Assets.Extensions;
using emt_sdk.Events;
using NLog;
using UnityEngine;

public class GalleryActionBindingComponent : MainThreadExecutorComponent
{
    private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

    [SerializeField]
    private GalleryPoolComponent _pool;

    [SerializeField]
    private GalleryCursorComponent _cursor;

    void Start()
    {
        EventManager.Instance.OnEffectCalled += OnEventReceived;
    }

    void OnDestroy()
    {
        EventManager.Instance.OnEffectCalled -= OnEventReceived;
    }

    private void OnEventReceived(EffectCall e)
    {
        var effect = e.Name.ToLowerInvariant();

        switch (effect)
        {
            case "left":
                ExecuteOnMainThread(() =>
                {
                    var count = e.Value ?? 1;
                    for (int i = 0; i < count; i++) _pool.Layout.Previous();
                });
            break;
            case "right":
                ExecuteOnMainThread(() =>
                {
                    var count = e.Value ?? 1;
                    for (int i = 0; i < count; i++) _pool.Layout.Next();
                });
                break;
            case "scrollTo":
                ExecuteOnMainThread(() =>
                {
                    int index = (int)e.Value.Value;
                    _pool.Layout.ScrollTo(index);
                });
                break;
            case "zoom":
                ExecuteOnMainThread(() =>
                {
                    int index = (int) e.Value.Value;

                    _pool.Layout.ScrollTo(index);
                    var gameObject = _pool.Layout.GetImage(index);

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

        Logger.InfoUnity($"[Gallery Action] {effect}: {e.Value?.ToString() ?? "void"}");
    }
}
