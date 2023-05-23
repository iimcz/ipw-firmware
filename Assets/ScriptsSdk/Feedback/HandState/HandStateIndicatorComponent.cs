using Naki3D.Common.Protocol;
using System;
using UnityEngine;
using static CameraHandMovement;

public class HandStateIndicatorComponent : MonoBehaviour
{
    [Min(0.01f)]
    public float InactivityDelay = 5.0f;

    [SerializeField]
    private GameObject _open;

    [SerializeField]
    private GameObject _closed;

    [SerializeField]
    private GameObject _pinch;

    [SerializeField]
    private GameObject _background;

    private float _lastActivity = 0f;
    private HandStateEnum _handState;

    void Update()
    {
        _lastActivity += Time.deltaTime;
        if (_lastActivity > InactivityDelay) _background.SetActive(false);
    }

    public void UpdateOpenHand(SensorDataMessage message)
    {
        _lastActivity = 0f;
        
        _background.SetActive(true);
        _closed.SetActive(false);
        _pinch.SetActive(false);
        _open.SetActive(true);
    }

    public void UpdateCloseHand(SensorDataMessage message)
    {
        _lastActivity = 0f;
        
        _background.SetActive(true);
        _open.SetActive(false);
        _closed.SetActive(true);
        _pinch.SetActive(false);
    }

    public void UpdatePinch(SensorDataMessage message)
    {
        _lastActivity = 0f;
        
        _background.SetActive(true);
        _open.SetActive(false);
        _closed.SetActive(false);
        _pinch.SetActive(true);
    }
}
