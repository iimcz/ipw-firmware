using Naki3D.Common.Protocol;

public class NameFilteredEventComponent : EventComponent
{
    public string EventName;

    protected override void OnEventReceived(SensorMessage e)
    {
        if (e.Data.Path != EventName) return;
        base.OnEventReceived(e);
    }
}
