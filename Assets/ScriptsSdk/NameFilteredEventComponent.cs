using emt_sdk.Generated.ScenePackage;
using Naki3D.Common.Protocol;
using UnityEngine;

public class NameFilteredEventComponent : EventComponent
{
    public string EventName;

    protected override void OnEventReceived(object sender, SensorMessage e)
    {
        if (e.SensorId != EventName) return;
        base.OnEventReceived(sender, e);
    }
}
