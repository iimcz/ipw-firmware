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

public class ExhibitConnectionComponent : MonoBehaviour
{
    public TcpClient Client;
    public ExhibitConnection Connection;
    public EmtSetting Settings;

    public static PackageDescriptor ActivePackage;

    public string Hostname;

    private PackageLoader _loader;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ApplyDelay());
        Settings = EmtSetting.FromConfig() ?? new EmtSetting
        {
            Type = Naki3D.Common.Protocol.DeviceType.Ipw,
            Communication = new CommunicationSettings(),
            PerformanceCap = PerformanceCap.Fast
        };
    }

    private void OnDestroy()
    {
        EventManager.Instance.Stop();
        Client.Close();
    }

    private IEnumerator ApplyDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        Client = new TcpClient();

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

    public void Connect(string hostname, int port)
    {
        Client.Connect(hostname, port);
        Connection = new ExhibitConnection(Client)
        {
            LoadPackageHandler = LoadPackage,
            ClearPackageHandler = pckg => { }
        };

        Connection.Connect();
        if (!Connection.Verified)
        {
            Debug.LogError("Failed to verify with server");
            return;
        }

        SendDescriptor();
        Debug.Log("Awaiting package");
    }

    void SendDescriptor()
    {
        var descriptor = new DeviceDescriptor
        {
            PerformanceCap = Settings.PerformanceCap,
            Type = Settings.Type
        };
        descriptor.LocalSensors.Add(SensorType.Gesture);
        Connection.SendDescriptor(descriptor);
    }

    void LoadPackage(LoadPackage pckg)
    {
        try
        {
            print("b");
            var package = _loader.LoadPackage(new StringReader(pckg.DescriptorJson), false);
            print("a");
            package.DownloadFile();
            print("c");
            Task.Run(() => EventManager.Instance.Start(package.Sync));

            SwitchScene(package);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        
    }

    public void SwitchScene(PackageDescriptor package)
    {
        ActivePackage = package;

        switch (ActivePackage.Parameters.DisplayType)
        {
            case "video":
                SceneManager.LoadScene("VideoScene");
                break;
            default:
                throw new NotImplementedException();
        };
    }
}
