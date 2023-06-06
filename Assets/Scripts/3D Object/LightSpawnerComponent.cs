using Unity.Burst.CompilerServices;
using UnityEngine;

public class LightSpawnerComponent : MonoBehaviour
{
    void Start()
    {
        var light = GetComponentInChildren<Light>();
        if (light != null) return; // Object is lit by creator

        var lightObject = new GameObject("DefaultLight");

        light = lightObject.AddComponent<Light>();
        light.color = Color.white;
        light.intensity = 1;
        light.type = LightType.Directional;

        lightObject.transform.parent = transform;
        lightObject.transform.position = new Vector3(0, 100, 0);
        lightObject.transform.eulerAngles = new Vector3(90, 0, 0);

        SkyboxLoader.ApplyAmbientLight(Color.white, 0.5f);
    }
}
