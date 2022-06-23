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
using NLog.Config;
using Assets.Extensions;

public class ExhibitConnectionComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static ExhibitConnection Connection;
    public EmtSetting Settings;

    public static PackageDescriptor ActivePackage;

    public string Hostname;

    public bool DisableAutoConnect;

    private PackageLoader _loader;
    private bool _changeScene;

    // Start is called before the first frame update
    void Start()
    {
        var nlogConfig = Path.Combine(Application.streamingAssetsPath, "NLog.config");
        NLog.LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);
        
        StartCoroutine(ApplyDelay());
        Settings = EmtSetting.FromConfig() ?? new EmtSetting
        {
            Type = Naki3D.Common.Protocol.DeviceType.Ipw,
            Communication = new CommunicationSettings(),
            PerformanceCap = PerformanceCap.Fast
        };
    }
    
    void Update()
    {
        if (!_changeScene) return;
        _changeScene = false;
        
        switch (ActivePackage.Parameters.DisplayType)
        {
            case "video":
                SceneManager.LoadScene("VideoScene");
                break;
            case "gallery":
                SceneManager.LoadScene("GalleryScene");
                break;
            case "model":
                SceneManager.LoadScene("3DObject");
                break;
            case "scene":
                ActivePackage.Run();
                break;
            case "panorama":
                SceneManager.LoadScene("PanoScene");
                break;
            default:
                throw new NotImplementedException(ActivePackage.Parameters.DisplayType);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.Stop();
        Connection?.Dispose();
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // TODO: Validate with schema
        _loader = new PackageLoader(null);

        if (string.IsNullOrWhiteSpace(Hostname)) Hostname = Dns.GetHostName();
        
        Task.Run(() => {
            var sync = new Sync
            {
                Elements = new System.Collections.Generic.List<Element>()
            };
            EventManager.Instance.Start(sync);
        });

        EventManager.Instance.OnEventReceived += Instance_OnEventReceived;

        if (!DisableAutoConnect)
        {
            Task.Run(() =>
            {
                Connect();
            });
        }
    }

    private void Instance_OnEventReceived(object sender, SensorMessage e)
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

        Task.Run(() => EventManager.Instance.Start(package.Sync));

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
        
        _changeScene = true;
    }
}
