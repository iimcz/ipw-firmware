using Naki3D.Common.Protocol;
using static Naki3D.Common.Protocol.SensorMessage;

public class TypeFilteredEventComponent : EventComponent
{
    public DataOneofCase DataType;

    protected override void OnEventReceived(object sender, SensorMessage e)
    {
        if (e.DataCase != DataType) return;
        base.OnEventReceived(sender, e);
    }
}
