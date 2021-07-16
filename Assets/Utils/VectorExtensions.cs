using UnityEngine;

public static class VectorExtensions
{
    public static Naki3D.Common.Protocol.Vector2 ToNakiVector(this Vector2 v)
    {
        return new Naki3D.Common.Protocol.Vector2
        {
            X = v.x,
            Y = v.y
        };
    }

    public static Naki3D.Common.Protocol.Vector3 ToNakiVector(this Vector3 v)
    {
        return new Naki3D.Common.Protocol.Vector3
        {
            X = v.x,
            Y = v.y,
            Z = v.z
        };
    }
}
