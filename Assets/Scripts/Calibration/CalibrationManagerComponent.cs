using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.Extensions;
using emt_sdk.Communication;
using emt_sdk.Communication.Discovery;
using emt_sdk.Packages;
using emt_sdk.ScenePackage;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using emt_sdk.Settings.IPW;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

// TODO: Write custom inspector for states to make it easier to read
public class CalibrationManagerComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public enum CalibrationStateEnum
    {
        Uninitialized,
        PhysicalAlignment,
        CornerAlignment,
        LensShift,
        ColorCorrection,
        Audio,
        Initialized
    }

    public enum NetworkStateEnum
    {
        Waiting,
        Valid,
        Invalid,
        VerificationDenied
    }

    public bool AlwaysCalibrate;

    private CalibrationStateEnum _calibrationState = CalibrationStateEnum.Uninitialized;
    private NetworkStateEnum _networkState = NetworkStateEnum.Waiting;
    
    [SerializeField] private ExhibitConnectionComponent _connection;
    [SerializeField] private NetworkComponent _network;
    [SerializeField] private AudioTestComponent _audioTest;

    // Unity can't serialize interfaces...
    private ICameraRig _camera;
    private DeviceTypeEnum _deviceType => _camera.DeviceType;
    private IConfigurationProvider<EMTSetting> _configProvider;

    public List<GameObject> UiStates;

    private void UpdateUi(CalibrationStateEnum state)
    {
        // Steps can be empty for different devices
        if (UiStates[(int)_calibrationState] != null) UiStates[(int)_calibrationState].SetActive(false);
        if (UiStates[(int)state] != null) UiStates[(int)state].SetActive(true);
        
        _calibrationState = state;
    }

    private IEnumerator WaitForConfigurationReset()
    {
        // Give the rendering pipeline a while to get itself sorted
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        var deviceType = _configProvider.Configuration.Type;
        var ipwConfigProvider = LevelScopeServices.Instance.GetRequiredService<IConfigurationProvider<IPWSetting>>();
        var isCalibrated = deviceType switch
        {
            DeviceTypeEnum.DEVICE_TYPE_PGE => true,
            DeviceTypeEnum.DEVICE_TYPE_IPW => ipwConfigProvider.ConfigurationExists,
            _ => true
        };

        if (AlwaysCalibrate) isCalibrated = false;
        if (isCalibrated)
        {
            // Give the user few seconds to react
            yield return new WaitForSecondsRealtime(5f); 
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) isCalibrated = false;
        }

        if (isCalibrated)
        {
            ProjectorTransformationPass.SoftwareCalibration = false;

            // TODO: implement alternative to broadcast - unicast with target address/hostname input
            var discovery = GlobalServices.Instance.GetService<IDiscoveryService>();
            discovery?.StartBroadcast();

            // TODO: Validate with schema
            var loader = new PackageLoader(null);
            var startupPackage = loader
                .EnumeratePackages(false)
                .FirstOrDefault(p => Path.GetFileName(p.PackageDirectory) == _connection.Settings.StartupPackage);
            
            if (startupPackage != null)
            {
                _connection.SwitchScene(startupPackage);
            }
        }
        else
        {
            ProjectorTransformationPass.SoftwareCalibration = true;
            UpdateUi(CalibrationStateEnum.PhysicalAlignment);
        }
    }

    private void Start()
    {
        _configProvider = GlobalServices.Instance.GetService<IConfigurationProvider<EMTSetting>>();
        StartCoroutine(WaitForConfigurationReset());
    }

    private void Update()
    {
        if (_camera == null)
        {
            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObject == null) return; // Waiting for camera spawn

            // Can't get an interface directly
            _camera = cameraObject.GetComponent<PeppersGhostCameraComponent>();
            if (_camera == null) _camera = cameraObject.GetComponent<DualCameraComponent>();
            if (_camera == null)
            {
                Logger.ErrorUnity("Main camera doesn't have an emt-sdk compatible display");
                throw new NotSupportedException("Main camera doesn't have an emt-sdk compatible display");
            }
        }

        switch (_calibrationState)
        {
            case CalibrationStateEnum.Uninitialized:
                break;
            case CalibrationStateEnum.PhysicalAlignment:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                if (_deviceType == DeviceTypeEnum.DEVICE_TYPE_PGE) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.CornerAlignment:
                ProjectorTransformationPass.EnableBlending = false;
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    UpdateUi(_calibrationState + 1);
                    ProjectorTransformationPass.EnableBlending = true;
                }
                if (_deviceType == DeviceTypeEnum.DEVICE_TYPE_PGE) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.ColorCorrection:
                if (Input.GetKeyDown(KeyCode.Return)) UpdateUi(_calibrationState + 1);
                if (_deviceType == DeviceTypeEnum.DEVICE_TYPE_PGE) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.LensShift:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    UpdateUi(_calibrationState + 1);
                    StartCoroutine(_audioTest.StartTest());
                    
                    _camera.SaveSettings(); // Save after changing all display related settings
                }
                if (_deviceType == DeviceTypeEnum.DEVICE_TYPE_PGE)
                {
                    StartCoroutine(_audioTest.StartTest());
                    UpdateUi(_calibrationState + 1);
                }
                break;
            case CalibrationStateEnum.Audio:
                if (_audioTest.Finished) UpdateUi(_calibrationState + 1);
                break;
            case CalibrationStateEnum.Initialized:
                ProjectorTransformationPass.SoftwareCalibration = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace) &&
            Input.GetKey(KeyCode.LeftShift) &&
            _calibrationState > CalibrationStateEnum.PhysicalAlignment &&
            _calibrationState <= CalibrationStateEnum.Initialized)
        {
            UpdateUi(_calibrationState - 1);
        }
    }
}
