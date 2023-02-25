using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class LevelScopeServices : MonoBehaviour
{
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
                    _instance = new GameObject(typeof(LevelScopeServices).Name).AddComponent<LevelScopeServices>();
                    _instance.CreateScope();
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
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
    }

    private void CreateScope()
    {
        _scope = GlobalServices.Instance.CreateScope();
    }
}