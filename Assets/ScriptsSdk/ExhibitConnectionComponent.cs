using emt_sdk.Communication;
using emt_sdk.Extensions;
using emt_sdk.ScenePackage;
using Naki3D.Common.Protocol;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ExhibitConnectionComponent : MonoBehaviour
{
    public TcpClient Client;
    public ExhibitConnection Connection;

    public string Hostname;
    public string TargetServer = "127.0.0.1";
    public int Port = 3917;

    private PackageLoader _loader;

    // Start is called before the first frame update
    void Start()
    {
        Client = new TcpClient();
        _loader = new PackageLoader(Path.Combine(Application.streamingAssetsPath, "package-schema.json"));

        if (string.IsNullOrWhiteSpace(Hostname)) Hostname = Dns.GetHostName();
    }

    public void Connect()
    {
        Client.Connect(TargetServer, Port);
        Connection = new ExhibitConnection(Client);
        Connection.Connect();
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
    }
}
