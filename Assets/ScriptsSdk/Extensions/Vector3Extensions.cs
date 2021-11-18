using Naki3D.Common.Protocol;

namespace Assets.ScriptsSdk.Extensions
{
    public static class Vector3Extensions
    {
        public static UnityEngine.Vector3 ToUnityVector(this Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }
    }
}
