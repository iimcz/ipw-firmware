using Naki3D.Common.Protocol;

public class NameFilteredEventComponent : EventComponent
{
    public string EventName;

    protected override void OnEventReceived(SensorDataMessage e)
    {
        if (e.Path != EventName) return;
        base.OnEventReceived(e);
    }
}
