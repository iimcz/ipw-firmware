using Assets.ScriptsSdk.Extensions;
using Naki3D.Common.Protocol;
using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraHandMovement : MonoBehaviour
{
    [SerializeField]
    private OrbitComponent _orbit;

    public enum HandStateEnum
    {
        None,
        Rotate,
        Zoom
    }


    [SerializeField]
    private HandStateEnum _handState = HandStateEnum.None;

    private Vector3 _previousPositon = Vector3.zero;
    private Vector3 _position = Vector3.zero;

    private Vector3 _delta => _position - _previousPositon;

    /// <summary>
    /// How long the script will wait until auto-orbit resumes
    /// </summary>
    [SerializeField]
    private float _autoOrbitDelay = 5f;

    /// <summary>
    /// Movement speed multiplier
    /// </summary>
    public float MovementSpeed = 10f;

    /// <summary>
    /// Zoom speed multiplier
    /// </summary>
    public float ZoomSpeed = 5f;

    private float _lastInputTime;

    void Start()
    {
        _lastInputTime = _autoOrbitDelay;
    }

    void Update()
    {
        // Only update if we have new data
        if (_lastInputTime >= _autoOrbitDelay) return;

        switch (_handState)
        {
            case HandStateEnum.None:
                break;
            case HandStateEnum.Rotate:
                _orbit.AdvanceAngle(_delta.x * MovementSpeed);
                _orbit.AdvancePivotRotation(new Vector3(0, 0, -_delta.y * MovementSpeed));
                break;
            case HandStateEnum.Zoom:
                _orbit.transform.position += _delta.z * ZoomSpeed * _orbit.transform.forward;
                break;
            default:
                throw new NotImplementedException("This hand state is not supported");
        }

        _lastInputTime += Time.deltaTime;
        if (_lastInputTime >= _autoOrbitDelay)
        {
            _orbit.AutoOrbit = true;
            _orbit.Invalidate(); // Reset to known position
        }
    }

    public void EventReceived(SensorMessage e)
    {
        if (e.DataCase != SensorMessage.DataOneofCase.HandTracking) return;

        _lastInputTime = 0f;
        _orbit.AutoOrbit = false;

        switch (e.HandTracking.Gesture)
        {
            case HandGestureType.GestureOpenHand:
                _handState = HandStateEnum.None;
                break;
            case HandGestureType.GestureCloseHand:
                _handState = HandStateEnum.Rotate;
                break;
            case HandGestureType.GesturePinch:
                _handState = HandStateEnum.Zoom;
                break;
        }

        _previousPositon = _position;
        _position = e.HandTracking.CenterPosition.ToUnityVector();
    }
}