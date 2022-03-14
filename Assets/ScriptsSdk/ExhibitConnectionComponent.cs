using System;
using System.Collections;
using emt_sdk.Communication;
using emt_sdk.Events;
using emt_sdk.ScenePackage;
using Naki3D.Common.Protocol;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using emt_sdk.Generated.ScenePackage;
using emt_sdk.Settings;
using NLog.Config;

public class ExhibitConnectionComponent : MonoBehaviour
{
    public ExhibitConnection Connection;
    public EmtSetting Settings;

    public static PackageDescriptor ActivePackage;

    public string Hostname;

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
            default:
                throw new NotImplementedException();
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
    }

    private void Instance_OnEventReceived(object sender, SensorMessage e)
    {
        Debug.Log(e);
    }

    public void Connect()
    {
        var descriptor = new DeviceDescriptor
        {
            PerformanceCap = Settings.PerformanceCap,
            Type = Settings.Type
        };
        descriptor.LocalSensors.Add(SensorType.Gesture);
        Connection = new ExhibitConnection(Settings.Communication, descriptor)
        {
            LoadPackageHandler = LoadPackage,
            ClearPackageHandler = pckg => { }
        };

        Connection.Connect();
    }

    void LoadPackage(LoadPackage pckg)
    {
        var package = _loader.LoadPackage(new StringReader(pckg.DescriptorJson), false);
        package.DownloadFile();
        Task.Run(() => EventManager.Instance.Start(package.Sync));

        Settings.StartupPackage = package.Package.Checksum;
        Settings.Save();
        
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
