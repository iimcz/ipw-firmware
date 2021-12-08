using emt_sdk.Scene;
using UnityEngine;

namespace Assets.ScriptsSdk.Extensions
{
    public static class GltfLocationExtensions
    {
        /// <summary>
        /// Attempts to find a unity GameObject by its GLTF name. Returns null if no such object is found.
        /// Only searches by name, locations with offset will always return null.
        /// </summary>
        /// <param name="location">GLTF location</param>
        /// <returns>Matching gameobject</returns>
        public static GameObject FindObject(this GltfObject.GltfLocation location)
        {
            if (string.IsNullOrEmpty(location.ObjectName)) return null;
            return GameObject.Find(location.ObjectName);
        }

        /// <summary>
        /// Finds a vector position corresponding to the location.
        /// Returns <see cref="Vector3.zero"/> if no object is found and offset is null.
        /// </summary>
        /// <param name="location">GLTF location</param>
        /// <returns>Position of matching gameobject</returns>
        public static Vector3 FindPosition(this GltfObject.GltfLocation location)
        {
            if (location.Offset != null) return location.Offset.ToUnityVector();
            var gameObject = FindObject(location);
            if (gameObject != null) return gameObject.transform.position;
            return Vector3.zero;
        }
    }
}