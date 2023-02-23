using Assets.Extensions;
using Google.Protobuf;
using Naki3D.Common.Protocol;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class ManagementMuterComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private TcpListener _listener;

    [SerializeField]
    private VideoPlayer _videoPlayer;

    [SerializeField]
    private MainThreadExecutorComponent _mainThreadExecutor;

    public void Start()
    {
        _listener = new TcpListener(IPAddress.Any, 5001);
        _listener.Start();

        Task.Run(() =>
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                Logger.InfoUnity($"Accepted management connection from {client.Client.RemoteEndPoint}");
                Task.Run(() => HandleConnection(client));
            }
        });
    }

    public void OnDestroy()
    {
        _listener.Stop();
    }

    private void HandleConnection(TcpClient client)
    {
        var stream = client.GetStream();

        // try
        // {
        //     var managementRequest = ManagementRequest.Parser.ParseDelimitedFrom(stream);
        //     if (managementRequest.ManagementType == ManagementRequest.Types.ManagementType.StartMute)
        //     {
        //         _mainThreadExecutor.ExecuteOnMainThread(() => _videoPlayer.SetDirectAudioMute(0, true));
        //         var response = new ManagementResponse
        //         {
        //             DeviceStatus = ManagementResponse.Types.DeviceStatus.Ok
        //         };

        //         Logger.InfoUnity("Management request - Muted audio");
        //         response.WriteDelimitedTo(stream);
        //     }
        //     else if (managementRequest.ManagementType == ManagementRequest.Types.ManagementType.Start)
        //     {
        //         _mainThreadExecutor.ExecuteOnMainThread(() => _videoPlayer.SetDirectAudioMute(0, false));
        //         var response = new ManagementResponse
        //         {
        //             DeviceStatus = ManagementResponse.Types.DeviceStatus.Ok
        //         };

        //         Logger.InfoUnity("Management request - Unmuted audio");
        //         response.WriteDelimitedTo(stream);
        //     }
        //     else
        //     {
        //         var response = new ManagementResponse
        //         {
        //             DeviceStatus = ManagementResponse.Types.DeviceStatus.Error
        //         };

        //         Logger.InfoUnity("Unhandled request - responding with error.");
        //         response.WriteDelimitedTo(stream);
        //     }
        // }
        // catch (Exception e)
        // {
        //     Logger.ErrorUnity("Invalid management message received, closing connection", e);
        // }
        // finally
        // {
        //     if (client.Connected) client.Close();
        // }
    }
}
