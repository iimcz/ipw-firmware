using Naki3D.Common.Protocol;

namespace Assets.ScriptsSdk.Extensions
{
    public static class Vector2Extensions
    {
        public static UnityEngine.Vector2 ToUnityVector(this Vector2Data v)
        {
            return new UnityEngine.Vector2(v.X, v.Y);
        }
    }
}
