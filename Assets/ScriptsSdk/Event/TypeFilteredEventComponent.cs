using Naki3D.Common.Protocol;
using static Naki3D.Common.Protocol.SensorMessage;

public class TypeFilteredEventComponent : EventComponent
{
    public DataOneofCase DataType;

    protected override void OnEventReceived(SensorMessage e)
    {
        if (e.DataCase != DataType) return;
        base.OnEventReceived(e);
    }
}
