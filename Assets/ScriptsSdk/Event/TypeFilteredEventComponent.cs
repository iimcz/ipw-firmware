using Naki3D.Common.Protocol;
using static Naki3D.Common.Protocol.SensorMessage;

public class TypeFilteredEventComponent : EventComponent
{
    public SensorDataMessage.DataOneofCase DataType;

    protected override void OnEventReceived(SensorDataMessage e)
    {
        if (e.DataCase != DataType) return;
        base.OnEventReceived(e);
    }
}
