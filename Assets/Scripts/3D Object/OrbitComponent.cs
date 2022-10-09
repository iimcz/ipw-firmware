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
    /// Position offset from target
    /// </summary>
    public Vector3 OrbitOffset;

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
        if (_pivot != null)
        {
            // Unparent camera to prevent destruction
            transform.parent = null;
            Destroy(_pivot);
        }
        
        // Create a new object on the origin point and parent ourself to it
        _pivot = new GameObject($"{gameObject.name} - Pivot");
        _pivot.transform.position = Origin;
        _pivot.transform.eulerAngles = Vector3.zero;

        transform.position = OrbitOffset;
        transform.parent = _pivot.transform;
    }

    /// <summary>
    /// Advances the rotation by a period of time in seconds (same speed as autorotate)
    /// </summary>
    /// <param name="time">Time period</param>
    public void AdvanceTime(float time)
    {
        _rotationProgress += time;
    }

    /// <summary>
    /// Advances the rotation by an angle in degrees
    /// </summary>
    /// <param name="angle">Rotation offset in degrees</param>
    public void AdvanceAngle(float angle)
    {
        var degreeDuration = RotationPeriod / 360f;
        var targetTime = angle * degreeDuration;

        _rotationProgress += targetTime;
    }

    public void AdvancePivotRotation(Vector3 eurlerAngles)
    {
        _pivot.transform.eulerAngles += eurlerAngles;
    }

    private void Update()
    {
        // Don't orbit until we have been initialized
        if (_pivot == null) return;
        if (LookAt == null) return;
        
        if (AutoOrbit) _rotationProgress += Time.deltaTime;

        // Clamp over/underflow
        if (_rotationProgress >= RotationPeriod) _rotationProgress -= RotationPeriod;
        if (_rotationProgress < 0) _rotationProgress += RotationPeriod;

        var progress = _rotationProgress / RotationPeriod;
        var angle = Mathf.Lerp(0, 360, progress);

        // TODO: Maybe use quaternions instead?
        var currentRotation = _pivot.transform.eulerAngles;
        _pivot.transform.eulerAngles = new Vector3(currentRotation.x, angle, currentRotation.z);
        
        transform.LookAt(LookAt.transform);
    }
}