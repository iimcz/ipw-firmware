using Naki3D.Common.Protocol;

public class FilteredEventComponent : EventComponent
{
    public string EventName;
    public SensorDataMessage.DataOneofCase DataType;

    protected override void OnEventReceived(SensorDataMessage e)
    {
        if (e.Path != EventName) return;
        if (e.DataCase != DataType) return;
        base.OnEventReceived(e);
    }
}
