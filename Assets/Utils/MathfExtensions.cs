using UnityEngine;

public static class MathfExtensions
{
    public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static Vector3 Map(this Vector3 value, Vector3 fromSource, Vector3 toSource, Vector3 fromTarget, Vector3 toTarget)
    {
        var xMap = value.x.Map(fromSource.x, toSource.x, fromTarget.x, toTarget.x);
        var yMap = value.y.Map(fromSource.y, toSource.y, fromTarget.y, toTarget.y);
        var zMap = value.z.Map(fromSource.z, toSource.z, fromTarget.z, toTarget.z);

        return new Vector3(xMap, yMap, zMap);
    }
}
