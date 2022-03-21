using UnityEngine;

public class CameraRigSpawnerComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject _dualCameraRig;

    [SerializeField]
    private GameObject _peppersGhostRig;

    void Start()
    {
        var config = emt_sdk.Settings.EmtSetting.FromConfig();
        if (config == null) Debug.LogError("Could not determine device type, not spawning any camera prefab");

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
