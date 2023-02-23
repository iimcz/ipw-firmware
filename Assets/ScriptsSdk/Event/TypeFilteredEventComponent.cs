using Naki3D.Common.Protocol;
using static Naki3D.Common.Protocol.SensorMessage;

public class TypeFilteredEventComponent : EventComponent
{
    public SensorDataMessage.DataOneofCase DataType;

    protected override void OnEventReceived(SensorMessage e)
    {
        if (e.Data.DataCase != DataType) return;
        base.OnEventReceived(e);
    }
}
