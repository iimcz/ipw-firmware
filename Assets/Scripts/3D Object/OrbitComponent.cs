using UnityEngine;

/// <summary>
/// Orbits a gameobject around a point
/// Reparents the rotating object!
/// </summary>
public class OrbitComponent : MonoBehaviour
{
    /// <summary>
    /// Rotation origin
    /// </summary>
    public Vector3 Origin;

    /// <summary>
    /// Look target
    /// </summary>
    public GameObject LookAt;
    
    /// <summary>
    /// Length of a single revolution in seconds
    /// </summary>
    public float RotationPeriod = 5f;

    /// <summary>
    /// Whether the object orbits automatically
    /// </summary>
    public bool AutoOrbit = true;

    private GameObject _pivot;
    private float _rotationProgress;

    public void Invalidate()
    {
        if (_pivot != null) Destroy(_pivot);
        
        // Create a new object on the origin point and parent ourself to it
        _pivot = new GameObject($"{gameObject.name} - Pivot");
        _pivot.transform.position = Origin;
        
        transform.parent = _pivot.transform;
    }

    public void Advance(float time)
    {
        _rotationProgress += time;
    }

    private void Update()
    {
        // Don|t orbit until we have been initialized
        if (_pivot == null) return;
        
        if (AutoOrbit) _rotationProgress += Time.deltaTime;
        if (_rotationProgress >= RotationPeriod) _rotationProgress -= RotationPeriod;

        var progress = _rotationProgress / RotationPeriod;
        var angle = Mathf.Lerp(0, 360, progress);

        // TODO: Maybe use quaternions instead?
        var currentRotation = _pivot.transform.eulerAngles;
        _pivot.transform.eulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);
        
        transform.LookAt(LookAt.transform);
    }
}
