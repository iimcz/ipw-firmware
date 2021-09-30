using emt_sdk.Communication;
using emt_sdk.Events;
using emt_sdk.Extensions;
using emt_sdk.ScenePackage;
using Naki3D.Common.Protocol;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExhibitConnectionComponent : MonoBehaviour
{
    public TcpClient Client;
    public ExhibitConnection Connection;

    public string Hostname;
    public string TargetServer = "127.0.0.1";
    public int Port = 3917;

    public bool DebugLoadTest;

    private PackageLoader _loader;

    // Start is called before the first frame update
    void Start()
    {
        Client = new TcpClient();
        _loader = new PackageLoader(Path.Combine(Application.streamingAssetsPath, "package-schema.json"));

        if (string.IsNullOrWhiteSpace(Hostname)) Hostname = Dns.GetHostName();

        Task.Run(() => {
            var sync = new emt_sdk.Generated.ScenePackage.Sync
            {
                Elements = new System.Collections.Generic.List<emt_sdk.Generated.ScenePackage.Element>()
            };
            EventManager.Instance.Start(sync);
        });

        EventManager.Instance.OnEventReceived += Instance_OnEventReceived;
    }

    private void Instance_OnEventReceived(object sender, SensorMessage e)
    {
        System.Console.WriteLine(e);
    }

    public void Connect()
    {
        if (DebugLoadTest)
        {
            SceneManager.LoadScene("Testing3DScene");
            return;
        }

        Client.Connect(TargetServer, Port);
        Connection = new ExhibitConnection(Client);
        Connection.LoadPackageHandler += LoadPackage;

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
            PerformanceCap = PerformanceCap.Fast,
            Type = Naki3D.Common.Protocol.DeviceType.Ipw
        };
        descriptor.LocalSensors.Add(SensorType.Gesture);
        Connection.SendDescriptor(descriptor);
    }

    void LoadPackage(LoadPackage pckg)
    {
        var package = _loader.LoadPackage(new StringReader(pckg.DescriptorJson), false);
        package.DownloadFile(".");
        EventManager.Instance.Start(package.Sync);

        switch (package.PackagePackage.Type)
        {
            case emt_sdk.Generated.ScenePackage.PackageType.Data:
                Debug.Log($"Downloaded data package '{package.PackagePackage.Type}'");
                break;
            case emt_sdk.Generated.ScenePackage.PackageType.Script:
                Debug.Log($"Downloaded script package '{package.PackagePackage.Type}'");
                break;
        }
    }
}
