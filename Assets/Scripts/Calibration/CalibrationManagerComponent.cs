using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class CalibrationManagerComponent : SerializedMonoBehaviour
{
    public enum CalibrationStateEnum
    {
        Uninitialized,
        PhysicalAlignment,
        CornerAlignment,
        OverlapCorrection,
        ColorCorrection,
        LensShift,
        NetworkConfiguration,
        NetworkCheck,
        Initialized
    }

    public enum NetworkStateEnum
    {
        Waiting,
        Valid,
        Invalid
    }

    public bool AlwaysCalibrate;
    
    private CalibrationStateEnum _calibrationState = CalibrationStateEnum.Uninitialized;
    private NetworkStateEnum _networkState = NetworkStateEnum.Waiting;
    
    [SerializeField] private ExhibitConnectionComponent _connection;
    [SerializeField] private NetworkComponent _network;
    [SerializeField] private DualCameraComponent _camera;

    [OdinSerialize]
    public Dictionary<CalibrationStateEnum, GameObject> UiStates { get; set; } =
        new SerializedDictionary<CalibrationStateEnum, GameObject>();

    private void UpdateUi(CalibrationStateEnum state)
    {
        UiStates[_calibrationState].SetActive(false);
        UiStates[state].SetActive(true);
        
        _calibrationState = state;
    }

    private IEnumerator WaitForConfigurationReset()
    {
        // Give the rendering pipeline a while to get itself sorted
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (AlwaysCalibrate)
        {
            UpdateUi(CalibrationStateEnum.PhysicalAlignment);
            yield break;
        }
        
        var isCalibrated = ProjectorTransfomartionSettingsLoader.SettingsExists;
        yield return new WaitForSecondsRealtime(5f); // Give the user few seconds to react
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) isCalibrated = false;

        if (isCalibrated)
        {
            Task.Run(VerifyNetwork);
            UpdateUi(CalibrationStateEnum.NetworkCheck);
        }
        else UpdateUi(CalibrationStateEnum.PhysicalAlignment);
    }

    private async Task VerifyNetwork()
    {
        await Task.Delay(2500);
        _networkState = NetworkStateEnum.Invalid;
    }

    private void Start()
    {
        StartCoroutine(WaitForConfigurationReset());
    }

    private void Update()
    {
        switch (_calibrationState)
        {
            case CalibrationStateEnum.Uninitialized:
                break;
            case CalibrationStateEnum.PhysicalAlignment:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.CornerAlignment:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.OverlapCorrection:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.ColorCorrection:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.LensShift:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.NetworkConfiguration:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    _camera.SaveSettings();

                    _networkState = NetworkStateEnum.Waiting;
                    Task.Run(VerifyNetwork);
                    
                    UpdateUi(_calibrationState + 1);
                }
                break;
            case CalibrationStateEnum.NetworkCheck:
                switch (_networkState)
                {
                    case NetworkStateEnum.Invalid:
                        _network.ShowWarning = true;
                        UpdateUi(_calibrationState - 1);
                        break;
                    case NetworkStateEnum.Valid:
                        UpdateUi(_calibrationState + 1);
                        break;
                }
                break;
            case CalibrationStateEnum.Initialized:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace) &&
            Input.GetKey(KeyCode.LeftShift) &&
            _calibrationState > CalibrationStateEnum.PhysicalAlignment &&
            _calibrationState <= CalibrationStateEnum.NetworkConfiguration)
        {
            UpdateUi(_calibrationState - 1);
        }

        // Checking the network again would just put us back where we were
        if (Input.GetKeyDown(KeyCode.Backspace) &&
            Input.GetKey(KeyCode.LeftShift) &&
            _calibrationState == CalibrationStateEnum.Initialized)
        {
            UpdateUi(CalibrationStateEnum.NetworkConfiguration);
        }
    }
}
