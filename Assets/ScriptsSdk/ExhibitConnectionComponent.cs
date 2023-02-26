using System;
using System.Collections;
using emt_sdk.Communication;
using emt_sdk.Events;
using emt_sdk.ScenePackage;
using Naki3D.Common.Protocol;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using emt_sdk.Events.Relay;
using emt_sdk.Settings;
using Assets.Extensions;
using Newtonsoft.Json;
using System.Linq;
using emt_sdk.Packages;
using emt_sdk.Settings.EMT;
using emt_sdk.Events.Local;

public class ExhibitConnectionComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    // These persist between scenes
    // public static ExhibitConnection Connection;
    public static PackageDescriptor ActivePackage;
    private static bool _changeScene;

    public EMTSetting Settings;
    public string Hostname;
    public bool AutoConnect;

    public bool LogEvents;
    private EventManager _eventManager;
    private ISensorManager _sensorManager;
    private IPackageRunner _runnerProxy;
    private SyncProvider _syncProvider;

    // private PackageLoader _loader;

    // Start is called before the first frame update
    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        _sensorManager = LevelScopeServices.Instance.GetRequiredService<ISensorManager>();
        
        _runnerProxy = LevelScopeServices.Instance.GetRequiredService<IPackageRunner>();
        if (_runnerProxy is PackageRunnerProxy)
        {
            ((PackageRunnerProxy)_runnerProxy).PackageRunAction = SwitchScene;
        }
        _syncProvider = LevelScopeServices.Instance.GetRequiredService<SyncProvider>();

        StartCoroutine(ApplyDelay());
    }
    
    void Update()
    {
        if (!_changeScene) return;
        _changeScene = false;
        
        switch (ActivePackage.Parameters.DisplayType)
        {
            case "video":
                SceneManager.LoadSceneAsync("VideoScene");
                break;
            case "gallery":
                SceneManager.LoadSceneAsync("GalleryScene");
                break;
            case "model":
                SceneManager.LoadSceneAsync("3DObject");
                break;
            case "scene":
                {
                    var relayServer = LevelScopeServices.Instance.GetRequiredService<EventRelayServer>();
                    Task.Run(() => relayServer.Listen());

                    // TODO: We want to wait for the TCP listener to initialize, but this is horrible
                    System.Threading.Thread.Sleep(500);

                    var process = ActivePackage.Run();
                    process.WaitForExit();
                    relayServer.TokenSource.Cancel();

                    Logger.ErrorUnity($"Scene package exitted with code '{process.ExitCode}', this probably shouldn't happen");
                    break;
                }
            case "panorama":
                SceneManager.LoadSceneAsync("PanoScene");
                break;
            default:
                Logger.ErrorUnity($"Package display type '{ActivePackage.Parameters.DisplayType}' is not implemented");
                throw new NotImplementedException($"Package display type '{ActivePackage.Parameters.DisplayType}' is not implemented");
        }
    }

    void OnApplicationQuit()
    {
        Logger.InfoUnity("Application quitting, stopping sensor manager");
        // EventManager.Instance.SensorManager.Stop();

        // TODO: Stop remote emt connections as well
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (string.IsNullOrWhiteSpace(Hostname)) Hostname = Dns.GetHostName();

        // if (EventManager.Instance.SensorManager == null || !EventManager.Instance.SensorManager.IsListening)
        // {
        //     Logger.InfoUnity("Application starting, starting sensor manager");
        //     EventManager.Instance.ConnectSensor(Settings.Communication);
        // }
        // TODO: check is sensor manager is already started (see previous implementation above)
        _eventManager.ConnectSensor();

        if (LogEvents) _eventManager.OnEventReceived += Instance_OnEventReceived;
    }

    private void Instance_OnEventReceived(SensorDataMessage e)
    {
        Logger.DebugUnity(e.ToString());
    }

    public void SwitchScene(PackageDescriptor package)
    {
        ActivePackage = package;

        _eventManager.Actions.Clear();
        _eventManager.Actions.AddRange(package.Inputs);

        if (!_eventManager.ConnectedRemote)
        {
            _eventManager.ConnectRemote(package.Sync);
        }

        // TODO: order of operations? event manager is not a singleton because sync provider isn't a singleton -> outgoing event connection isn't a singleton

        //     EventManager.Instance.Actions.Clear();
        //     EventManager.Instance.Actions.AddRange(package.Inputs);

        //     if (!EventManager.Instance.ConnectedRemote)
        //     {
        //         EventManager.Instance.ConnectRemote(package.Sync, Settings.Communication);
        //     }

        //     _changeScene = true;
    }
}
