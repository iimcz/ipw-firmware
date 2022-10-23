using Assets.Extensions;
using emt_sdk.Events.NtpSync;
using Naki3D.Common.Protocol;
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

    public NtpScheduler Scheduler { get; private set; }

    void Start()
    {
        var config = emt_sdk.Settings.EmtSetting.FromConfig();

        if (config.Communication.NtpHostname == null) Scheduler = new NtpScheduler();
        else Scheduler = new NtpScheduler(config.Communication.NtpHostname);

        StartCoroutine(ResyncTime());
    }

    IEnumerator ResyncTime()
    {
        while (true)
        {
            var task = Scheduler.Resync();
            yield return new WaitUntil(() => task.IsCompleted);
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

    public void OnCustomEvent(SensorMessage message)
    {
        if (!enabled) return;

        switch (message.Event.Name)
        {
            case "VideoPlayer_ScheduleStart":
                ScheduleStart(DateTime.Parse(message.Event.Parameters));
                break;
        }
    }
}
