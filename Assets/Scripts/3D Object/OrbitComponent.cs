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
    public Transform Origin;

    /// <summary>
    /// Length of a single revolution in seconds
    /// </summary>
    public float RotationPeriod = 5f;

    private GameObject _pivot;
    private float _rotationProgress;

    private void Start()
    {
        // Create a new object on the origin point and parent ourself to it
        _pivot = new GameObject($"{gameObject.name} - Pivot");
        _pivot.transform.position = Origin.position;

        transform.parent = _pivot.transform;
    }

    private void Update()
    {
        _rotationProgress += Time.deltaTime;
        if (_rotationProgress >= RotationPeriod) _rotationProgress -= RotationPeriod;

        var progress = _rotationProgress / RotationPeriod;
        var angle = Mathf.Lerp(0, 360, progress);

        // TODO: Maybe use quaternions instead?
        var currentRotation = _pivot.transform.eulerAngles;
        _pivot.transform.eulerAngles = new Vector3(currentRotation.x, angle, currentRotation.y);
    }
}