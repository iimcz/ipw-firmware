using System;
using UnityEngine;
using Microsoft.Extensions.DependencyInjection;

using emt_sdk.Communication;
using emt_sdk.Events;
using emt_sdk.Packages;
using emt_sdk.Scene;
using emt_sdk.ScenePackage;
using emt_sdk.Settings;

public class GlobalServices : MonoBehaviour
{
    private static readonly object _lock;
    private static GlobalServices _instance;
    public static GlobalServices Instance
    {
        get {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameObject(typeof(GlobalServices).Name).AddComponent<GlobalServices>();
                }
                return _instance;
            }
        }
    }

    public ServiceProvider ServiceProvider { get; private set; }

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);

        initializeServices();
    }

    private void initializeServices()
    {
        var services = new ServiceCollection();

        services.AddJsonFileSettings();

        services.AddGrpcExhibitConnection();

        ServiceProvider = services.BuildServiceProvider();
    }
}