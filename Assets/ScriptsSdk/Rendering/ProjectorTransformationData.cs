using UnityEngine;

public class ProjectorTransformationData
{
    public Mesh ScreenMesh = MeshUtils.CreateTransform(new Vector3[] { new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), new Vector3(1, 1, 0), new Vector3(1, -1, 0) });
    public float Saturation = 1f;
    public float Brightness = 0f;
    public float Contrast = 1f;
    
    /// <summary>
    /// Flips all transformations for use on mirrored screen
    /// </summary>
    public bool FlipCurve = false;

    /// <summary>
    /// Determines the amount of darkened crossover space in the middle of the IPW
    /// </summary>
    public float CrossOver = 0.05f;
    
    /// <summary>
    /// Enables dimming and anti-distortion ransformations
    /// </summary>
    public bool EnableCurve = true;
}
