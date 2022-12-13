﻿using UnityEngine;

/// <summary>
/// Orbits a gameobject around a point
/// Reparents the rotating object!
/// </summary>
public class OrbitComponent : MonoBehaviour
{
    /// <summary>
    /// Rotation origin
    /// </summary>
    [Header("Origin")]
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
    [Header("Rotation")]
    public float RotationPeriod = 5f;

    /// <summary>
    /// Minimal allowed orbit postion longitude. -90 corresponds to the camera looking straight up from the bottom of the model if height is 0;
    /// </summary>
    public float LongitudeMin = -85;

    /// <summary>
    /// Maximal allowed orbit postion longitude. 90 corresponds to the camera looking straight down from the top of the model if height is 0;
    /// </summary>
    public float LongitudeMax = 85;

    /// <summary>
    /// Whether the object orbits automatically
    /// </summary>
    public bool AutoOrbit = true;

    private GameObject _pivot;

    /// <summary>
    /// Camera position on rotation sphere
    /// </summary>
    private float _latitude, _longitude;

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
        transform.LookAt(LookAt.transform);
    }

    /// <summary>
    /// Advances the rotation by a period of time in seconds (same speed as autorotate)
    /// </summary>
    /// <param name="time">Time period</param>
    public void AdvanceTime(float time)
    {
        _rotationProgress += time;
    }

    public void ResetTime()
    {
        _rotationProgress = 0f;
    }

    /// <summary>
    /// Advances the rotation by an angle in degrees
    /// </summary>
    /// <param name="angle">Rotation offset in degrees</param>
    public void AdvanceAngle(float latitude, float longitude)
    {
        _latitude += latitude;
        _longitude += longitude;
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

        // Prevent from flipping over
        _longitude = Mathf.Clamp(_longitude, LongitudeMin, LongitudeMax);

        _pivot.transform.rotation = Quaternion.AngleAxis(_latitude + angle, Vector3.up) * Quaternion.AngleAxis(_longitude, Vector3.forward);
    }
}