using Assets.Extensions;
using UnityEngine;

public class CameraRigSpawnerComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    [SerializeField]
    private GameObject _dualCameraRig;

    [SerializeField]
    private GameObject _peppersGhostRig;

    void Start()
    {
        var config = emt_sdk.Settings.EmtSetting.FromConfig();
        if (config == null) Logger.ErrorUnity("Could not determine device type, not spawning any camera prefab");

        switch (config.Type)
        {
            case Naki3D.Common.Protocol.DeviceType.Ipw:
                Instantiate(_dualCameraRig);
                break;
            case Naki3D.Common.Protocol.DeviceType.Pge:
                Instantiate(_peppersGhostRig);
                break;
        }
    }
}
