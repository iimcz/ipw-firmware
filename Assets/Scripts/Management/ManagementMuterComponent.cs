using Assets.Extensions;
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

    [SerializeField]
    private VideoPlayer _videoPlayer;

    [SerializeField]
    private MainThreadExecutorComponent _mainThreadExecutor;
    private DeviceService.DeviceServiceBase _deviceService;

    public void Start()
    {
        _deviceService = LevelScopeServices.Instance.GetRequiredService<DeviceService.DeviceServiceBase>();
        _deviceService.VolumeChanged += HandleVolumeChanged;
    }

    private void HandleVolumeChanged(float volume)
    {
        _mainThreadExecutor.ExecuteOnMainThread(() =>
        {
            _videoPlayer.SetDirectAudioVolume(0, volume);
        });
    }

    public void OnDestroy()
    {
        _deviceService.VolumeChanged -= HandleVolumeChanged;
    }
}
