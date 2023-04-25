using System.Collections.Generic;
using emt_sdk.Events;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class LevelScopeServices : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private static readonly object _lock = new object();
    private static LevelScopeServices _instance;
    private IServiceScope _scope;

    public static LevelScopeServices Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    // Awake gets called synchronously, _instance will be set after this line
                    new GameObject(typeof(LevelScopeServices).Name).AddComponent<LevelScopeServices>();
                }
                return _instance;
            }
        }
    }

    public T GetService<T>()
    {
        return _scope.ServiceProvider.GetService<T>();
    }

    public T GetRequiredService<T>()
    {
        return _scope.ServiceProvider.GetRequiredService<T>();
    }

    public IEnumerable<T> GetServices<T>()
    {
        return _scope.ServiceProvider.GetServices<T>();
    }

    public void Awake()
    {
        // We are replacing the current level scope, old object is already destroyed by Unity
        // Assuming we don't create two local scopes in the same scene
        CreateScope();
        _instance = this;
    }

    public void OnDestroy()
    {
        Logger.Info("Disposing level-scoped services.");
        var eventManager = GetRequiredService<EventManager>();
        eventManager.Dispose();

        // Make sure we don't grab an invalid reference from Awake that got exectuted before the
        // Awake of the replacement
        _instance = null;
    }

    private void CreateScope()
    {
        _scope = GlobalServices.Instance.CreateScope();
    }
}