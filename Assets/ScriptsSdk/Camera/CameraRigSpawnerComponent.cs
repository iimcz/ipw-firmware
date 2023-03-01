using Assets.Extensions;
using emt_sdk.Settings;
using emt_sdk.Settings.EMT;
using UnityEngine;

public class CameraRigSpawnerComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public  ICameraRig CameraRig { get; private set; }

    private GameObject _spawnedRig;

    [SerializeField]
    private GameObject _dualCameraRig;

    [SerializeField]
    private GameObject _peppersGhostRig;

    void Start()
    {
        emt_sdk.Settings.EMT.EMTSetting config = GlobalServices.Instance.GetRequiredService<IConfigurationProvider<EMTSetting>>().Configuration;
        if (config == null) Logger.ErrorUnity("Could not determine device type, not spawning any camera prefab");

        switch (config.Type)
        {
            case emt_sdk.Settings.EMT.DeviceTypeEnum.DEVICE_TYPE_IPW:
                _spawnedRig = Instantiate(_dualCameraRig, transform);
                CameraRig = _spawnedRig.GetComponent<DualCameraComponent>();
                break;
            case emt_sdk.Settings.EMT.DeviceTypeEnum.DEVICE_TYPE_PGE:
                _spawnedRig = Instantiate(_peppersGhostRig, transform);
                CameraRig = _spawnedRig.GetComponent<PeppersGhostCameraComponent>();
                break;
        }
    }
}
