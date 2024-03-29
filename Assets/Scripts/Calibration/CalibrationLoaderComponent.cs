using Assets.Extensions;
using emt_sdk.Settings;
using NLog.Config;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using DeviceType = Naki3D.Common.Protocol.DeviceType;

public class CalibrationLoaderComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public void Start()
    {
        var nlogConfig = Path.Combine(Application.streamingAssetsPath, "NLog.config");
        NLog.LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);

        var settings = EmtSetting.FromConfig();
        if (settings == null) new EmtSetting { Type = DeviceType.Unknown }.Save();
        
        var deviceType = settings?.Type ?? DeviceType.Unknown;

        switch (deviceType)
        {
            case Naki3D.Common.Protocol.DeviceType.Ipw:
                SceneManager.LoadScene("EmtToolboxScene");
                break;
            case Naki3D.Common.Protocol.DeviceType.Pge:
                SceneManager.LoadScene("EmtToolboxScenePge");
                break;
            case Naki3D.Common.Protocol.DeviceType.Unknown:
            default:
                Logger.ErrorUnity($"Device type '{deviceType}' is not supported.");
                throw new NotImplementedException($"Device type '{deviceType}' is not supported.");
        }
    }
}
