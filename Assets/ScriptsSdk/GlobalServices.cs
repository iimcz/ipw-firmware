using System;
using UnityEngine;
using Microsoft.Extensions.DependencyInjection;

using emt_sdk.Communication;
using emt_sdk.Events;
using emt_sdk.Packages;
using emt_sdk.Scene;
using emt_sdk.ScenePackage;
using emt_sdk.Settings;
using System.Collections.Generic;

public class GlobalServices : MonoBehaviour
{
    private static readonly object _lock = new object();
    private static GlobalServices _instance;
    public static GlobalServices Instance
    {
        get {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(GlobalServices).Name).AddComponent<GlobalServices>();
                    _instance.InitializeServices();
                }
                return _instance;
            }
        }
    }

    private ServiceProvider _serviceProvider;

    public T GetService<T>()
    {
        return _serviceProvider.GetService<T>();
    }

    public T GetRequiredService<T>()
    {
        return _serviceProvider.GetRequiredService<T>();
    }

    public IEnumerable<T> GetServices<T>()
    {
        return _serviceProvider.GetServices<T>();
    }

    public IServiceScope CreateScope()
    {
        return _serviceProvider.CreateScope();
    }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        if (_serviceProvider == null)
        {
            InitializeServices();
        }

        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void InitializeServices()
    {
        var services = new ServiceCollection();

        services.AddJsonFileSettings();
        services.AddGrpcExhibitConnection();
        services.AddUDPDiscovery();

        services.AddLocalPackages();
        services.AddEvents();
        
        services.AddNTPSynchronization();
        services.AddSensorProjectorControl();

        services.AddPackageRunnerProxy();
        services.AddSyncConfiguration();

        _serviceProvider = services.BuildServiceProvider();
    }
}