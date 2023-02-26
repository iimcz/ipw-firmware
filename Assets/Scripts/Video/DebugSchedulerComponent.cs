using emt_sdk.Events;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class DebugSchedulerComponent : MonoBehaviour
{
    [SerializeField]
    private NtpVideoSyncComponent _sync;
    private EventManager _eventManager;

    void Awake()
    {
        _eventManager = LevelScopeServices.Instance.GetRequiredService<EventManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var targetTime = _sync.Scheduler.SynchronizedTime + TimeSpan.FromSeconds(1);
            var message = new Naki3D.Common.Protocol.SensorDataMessage
            {
                Path = "VideoPlayer_ScheduleStart",
                String = targetTime.ToString()
            };

            _sync.ScheduleStart(targetTime);
            if (_eventManager.ConnectedRemote) _eventManager.BroadcastEvent(message);
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            var resync = _sync.GenerateResyncMessage();
            var message = new Naki3D.Common.Protocol.SensorDataMessage
            {
                Path = "VideoPlayer_ScheduleResync",
                String = JsonConvert.SerializeObject(resync)
            };

            if (_eventManager.ConnectedRemote) _eventManager.BroadcastEvent(message);
        }
    }
}
