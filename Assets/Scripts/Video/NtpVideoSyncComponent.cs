using Assets.Extensions;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using emt_sdk.Events;
using emt_sdk.Events.NtpSync;
using Naki3D.Common.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// TODO: Look into VideoPlayer.externalReferenceTime, but docs are sparse and forum users claim it's not good

public class NtpVideoSyncComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// How often (in seconds) the component will resync time with the NTP server
    /// </summary>
    public float ResyncPeriod = 30f;

    [SerializeField]
    private VideoPlayer _player;
    private EventManager _eventManager;

    public NtpScheduler Scheduler { get; private set; }

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
        var config = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<EMTSetting>>();

        if (config.Configuration.Communication.NtpHostname == null) Scheduler = new NtpScheduler();
        else Scheduler = new NtpScheduler(config.Configuration.Communication.NtpHostname);

        StartCoroutine(ResyncTime());
    }

    IEnumerator ResyncTime()
    {
        while (true)
        {
            var task = Scheduler.Resync();
            yield return new WaitUntil(() => task.IsCompleted);
            SendResync();
            yield return new WaitForSecondsRealtime(ResyncPeriod);
        }
    }

    void Update()
    {
        Scheduler.RunActions();
    }

    public void ScheduleStart(DateTime scheduledTime)
    {
        // TODO: Not very nice
        // Prevent duplicant actions, only handle last
        try
        {
            Scheduler.RemoveAction("VideoPlayer_ScheduleStart");
        }
        catch { }

        Logger.InfoUnity($"Scheduled video start for {scheduledTime}");
        var playAction = new NtpAction(scheduledTime, () =>
        {
            Logger.InfoUnity("Starting scheduled video");
            _player.Play();
        }, "VideoPlayer_ScheduleStart");
        Scheduler.ScheduleAction(playAction);
    }

    public void ScheduleResync(DateTime scheduledTime, double seekTime)
    {
        // TODO: Not very nice
        // Prevent duplicant actions, only handle last
        try
        {
            Scheduler.RemoveAction("VideoPlayer_ScheduleResync");
        }
        catch { }

        Logger.InfoUnity($"Scheduled video resync for {scheduledTime}, seeking to {seekTime}");
        var playAction = new NtpAction(scheduledTime, () =>
        {
            Logger.InfoUnity("Resyncing scheduled video");
            _player.time = seekTime;
        }, "VideoPlayer_ScheduleResync");
        Scheduler.ScheduleAction(playAction);
    }

    public VideoResyncParameters GenerateResyncMessage()
    {
        return new VideoResyncParameters
        {
            ScheduledTime = Scheduler.SynchronizedTime + TimeSpan.FromSeconds(1),
            SeekTime = _player.time + 1f
        };
    }

    public void SendResync()
    {
        var resync = GenerateResyncMessage();
        var message = new Naki3D.Common.Protocol.SensorDataMessage
        {
            Path = "VideoPlayer_ScheduleResync",
            String = JsonConvert.SerializeObject(resync)
        };

        if (_eventManager.ConnectedRemote) _eventManager.BroadcastEvent(message);
    }

    public void ScheduleStartEvent(SensorDataMessage message)
    {
        ScheduleStart(DateTime.Parse(message.String));
    }

    public void ScheduleResyncEvent(SensorDataMessage message)
    {
        var parameters = JsonConvert.DeserializeObject<VideoResyncParameters>(message.String);
        ScheduleResync(parameters.ScheduledTime, parameters.SeekTime);
    }
}
