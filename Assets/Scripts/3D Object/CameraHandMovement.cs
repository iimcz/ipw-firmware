using Assets.ScriptsSdk.Extensions;
using Naki3D.Common.Protocol;
using System;
using UnityEngine;

using Quaternion = UnityEngine.Quaternion;
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

    private KalmanFilter _filterX = new KalmanFilter(1, 1, 0.125, 1, 0.1, 0);
    private KalmanFilter _filterY = new KalmanFilter(1, 1, 0.125, 1, 0.1, 0);
    private KalmanFilter _filterZ = new KalmanFilter(1, 1, 0.125, 1, 0.1, 0);

    private Vector3 _previousPositon = Vector3.zero;
    private Vector3 _position = Vector3.zero;

    [SerializeField]
    private Vector3 _minSourceRange, _maxSourceRange;

    [SerializeField]
    private Vector3 _minTargetRange, _maxTargetRange;

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
        if (_lastInputTime >= _autoOrbitDelay)
        {
            // Maybe have a separate anti-spin delay?
            _position = Vector3.zero;
            _previousPositon = Vector3.zero;
            return;
        }

        switch (_handState)
        {
            case HandStateEnum.None:
                break;
            case HandStateEnum.Rotate:
                _orbit.AdvanceAngle(_delta.x * MovementSpeed);

                var rotation = Quaternion.AngleAxis(-_delta.y * MovementSpeed, Vector3.forward);
                _orbit.AdvancePivotRotation(rotation);
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
            //_orbit.AutoOrbit = true;
            //_orbit.Invalidate(); // Reset to known position
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

        // Prevent jump in case it's out first event
        if (_previousPositon == Vector3.zero) _previousPositon = _position;

        _position = new Vector3(
            _filterX.Output(_position.x),
            _filterY.Output(_position.y),
            _filterZ.Output(_position.z)
        );

        _position = _position.Map(_minSourceRange, _maxSourceRange, _minTargetRange,_maxTargetRange);
    }
}