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

    private float _lastActivity = 0f;
    private HandStateEnum _handState;

    void Update()
    {
        _lastActivity += Time.deltaTime;
        if (_lastActivity > InactivityDelay) gameObject.SetActive(false);
    }

    public void UpdateHand(SensorDataMessage message)
    {
        if (!Enum.TryParse<HandStateEnum>(message.String, true, out _handState))
        {
            Debug.Log("Tried to activate an invalid camera mode");
            return;
        }

        _lastActivity = 0f;
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        _open.SetActive(false);
        _closed.SetActive(false);
        _pinch.SetActive(false);

        switch (_handState)
        {
            case HandStateEnum.Rotate:
                _open.SetActive(true);
                break;
            case HandStateEnum.Zoom:
                _pinch.SetActive(true);
                break;
            case HandStateEnum.None:
                _open.SetActive(true);
                break;
        }
    }
}
