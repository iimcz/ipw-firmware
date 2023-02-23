using Assets.Extensions;
using emt_sdk.Events;
using emt_sdk.Events.NtpSync;
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

    public NtpScheduler Scheduler { get; private set; }

    void Start()
    {
        var config = new emt_sdk.Settings.EMT.EMTSetting();
        //var config = emt_sdk.Settings.EMT.EMTSetting.EmtSetting.FromConfig();

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
        var message = new Naki3D.Common.Protocol.SensorMessage
        {
            Data = new SensorDataMessage
            {
                Timestamp = (ulong)System.DateTime.UtcNow.Ticks,
                Path = "3DObject_ScheduleReset",
                String = targetTime.ToString()
            }
        };

        ScheduleReset(targetTime);
        //if (EventManager.Instance.ConnectedRemote) EventManager.Instance.BroadcastEvent(message);
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

    public void OnCustomEvent(SensorMessage message)
    {
        if (!enabled) return;

        // TODO: Add hostname parameter
        // switch (message.Event.Name)
        // {
        //     case "3DObject_ScheduleReset":
        //         ScheduleReset(DateTime.Parse(message.Event.Parameters));
        //         break;
        // }
    }
}
