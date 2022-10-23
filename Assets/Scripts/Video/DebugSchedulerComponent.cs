using emt_sdk.Events;
using System;
using UnityEngine;

public class DebugSchedulerComponent : MonoBehaviour
{
    [SerializeField]
    private NtpVideoSyncComponent _sync;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var targetTime = _sync.Scheduler.SynchronizedTime + TimeSpan.FromSeconds(1);
            var message = new Naki3D.Common.Protocol.SensorMessage
            {
                Event = new Naki3D.Common.Protocol.EventData
                {
                    Name = "VideoPlayer_ScheduleStart",
                    Parameters = targetTime.ToString()
                }
            };

            _sync.ScheduleStart(targetTime);
            if (EventManager.Instance.ConnectedRemote) EventManager.Instance.BroadcastEvent(message);
        }
    }
}
