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
using emt_sdk.Generated.ScenePackage;
using emt_sdk.Settings;
using Assets.Extensions;
using System.Collections.Generic;

public class ExhibitConnectionComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    // These persist between scenes
    public static ExhibitConnection Connection;
    public static PackageDescriptor ActivePackage;
    private static bool _changeScene;

    public EmtSetting Settings;
    public string Hostname;
    public bool AutoConnect;

    public bool LogEvents;

    private PackageLoader _loader;

    // Start is called before the first frame update
    void Start()
    {      
        StartCoroutine(ApplyDelay());

        Settings = EmtSetting.FromConfig();
        if (Settings == null)
        {
            Logger.ErrorUnity("Attempted to create ExhibitConnectionComponent without a valid EMT config, assuming defaults");
            Settings = new EmtSetting();
        }
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
                ActivePackage.Run();
                break;
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
        EventManager.Instance.SensorManager.Stop();

        // TODO: Stop remote emt connections as well
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // TODO: Validate with schema
        _loader = new PackageLoader(null);

        if (string.IsNullOrWhiteSpace(Hostname)) Hostname = Dns.GetHostName();

        if (EventManager.Instance.SensorManager == null || !EventManager.Instance.SensorManager.IsListening)
        {
            Logger.InfoUnity("Application starting, starting sensor manager");
            EventManager.Instance.ConnectSensor(Settings.Communication);
        }
        if (LogEvents) EventManager.Instance.OnEventReceived += Instance_OnEventReceived;

        if (AutoConnect)
        {
            if (EmtSetting.FromConfig() == null) Logger.ErrorUnity($"Attempted to connect to toolbox without a valid config, ignoring");
            else Connect();
        }
    }

    private void Instance_OnEventReceived(SensorMessage e)
    {
        Logger.DebugUnity(e.ToString());
    }

    public void Connect()
    {
        var descriptor = new DeviceDescriptor
        {
            PerformanceCap = Settings.PerformanceCap,
            Type = Settings.Type
        };
        descriptor.LocalSensors.Add(SensorType.Gesture);

        try
        {
            Connection = new ExhibitConnection(Settings.Communication, descriptor)
            {
                LoadPackageHandler = LoadPackage,
                ClearPackageHandler = pckg => { }
            };
        }
        catch (SocketException e)
        {
            // DNS resolve fail, abort
            Logger.ErrorUnity("Failed to estabilish exhibit connection, aborting", e);
            return;
        }
        

        Connection.Connect();
    }

    void LoadPackage(LoadPackage pckg)
    {
        var package = _loader.LoadPackage(new StringReader(pckg.DescriptorJson), false);
        Logger.InfoUnity($"Starting file download for package '{package.Metadata.PackageName}'");
        package.DownloadFile();
        Logger.InfoUnity($"Download complete");

        EventManager.Instance.ConnectRemote(package.Sync, Settings.Communication);

        Settings.StartupPackage = package.Package.Checksum;
        Settings.Save();

        Logger.InfoUnity($"Switching scene to package '{package.Metadata.PackageName}'");
        SwitchScene(package);
    }

    public void SwitchScene(PackageDescriptor package)
    {
        ActivePackage = package;
        
        EventManager.Instance.Actions.Clear();
        EventManager.Instance.Actions.AddRange(package.Inputs);

        EventManager.Instance.ConnectRemote(package.Sync, Settings.Communication);
        
        _changeScene = true;
    }
}
