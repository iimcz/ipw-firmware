using Naki3D.Common.Protocol;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector2Data ToNakiVector(this Vector2 v)
    {
        return new Vector2Data
        {
            X = v.x,
            Y = v.y
        };
    }

    public static Vector3Data ToNakiVector(this Vector3 v)
    {
        return new Vector3Data
        {
            X = v.x,
            Y = v.y,
            Z = v.z
        };
    }
}
