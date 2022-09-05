using Newtonsoft.Json;
using Siccity.GLTFUtility.Converters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Siccity.GLTFUtility
{
    // https://github.com/KhronosGroup/glTF/blob/main/extensions/2.0/Khronos/KHR_lights_punctual/README.md
    [Preserve] public class KHR_lights_punctual
    {
        public List<GLTFLight> lights;

        public class GLTFLight
        {
            public string name = "";
            [JsonConverter(typeof(ColorRGBConverter))] public Color color = Color.white;
            public float intensity = 1.0f;
            [JsonProperty(Required = Required.Always), JsonConverter(typeof(EnumConverter))] public LightType type;
            public float range;

            // spot light
            public float innerConeAngle = 0f;
            public float outerConeAngle = Mathf.PI / 4f;
        }
    }
}
