using Assets.Extensions;
using emt_sdk.Events;
using emt_sdk.Events.NtpSync;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using Microsoft.Extensions.DependencyInjection;
using Naki3D.Common.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class Ntp3DObjectSyncComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// How often (in seconds) the component will resync time with the NTP server
    /// </summary>
    public float ResyncPeriod = 30f;

    [SerializeField]
    private OrbitComponent _orbit;
    private EventManager _eventManager;

    public NtpScheduler Scheduler { get; private set; }

    void Start()
    {
        _eventManager = LevelScopeServices.Instance.GetService<EventManager>();

        var configProvider = LevelScopeServices.Instance.GetService<IConfigurationProvider<EMTSetting>>();
        var config = configProvider.Configuration;

        if (config.Communication.NtpHostname == null) Scheduler = new NtpScheduler();
        else Scheduler = new NtpScheduler(config.Communication.NtpHostname);

        StartCoroutine(ResyncRotation());
    }

    IEnumerator ResyncRotation()
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

    public void SendReset()
    {
        var targetTime = Scheduler.SynchronizedTime + TimeSpan.FromSeconds(1);
        var message = new SensorDataMessage
        {
            Timestamp = (ulong)System.DateTime.UtcNow.Ticks,
            Path = "3DObject_ScheduleReset",
            String = targetTime.ToString()
        };

        ScheduleReset(targetTime);
        if (_eventManager.ConnectedRemote) _eventManager.BroadcastEvent(message);
    }

    public void ScheduleReset(DateTime scheduledTime)
    {
        // TODO: Not very nice
        // Prevent duplicant actions, only handle last
        try
        {
            Scheduler.RemoveAction("3DObject_ScheduleReset");
        }
        catch { }

        Logger.InfoUnity($"Scheduled object resync for {scheduledTime}");
        var resetAction = new NtpAction(scheduledTime, () =>
        {
            Logger.InfoUnity("Resyncing 3D object");
            _orbit.ResetTime();
        }, "3DObject_ScheduleReset");
        Scheduler.ScheduleAction(resetAction);
    }

    public void OnCustomEvent(SensorDataMessage message)
    {
        if (!enabled) return;
        ScheduleReset(DateTime.Parse(message.String));
    }
}
