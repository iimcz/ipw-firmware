using Assets.Extensions;
using emt_sdk.Settings;
using NLog.Config;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Microsoft.Extensions.DependencyInjection;
using emt_sdk.Settings.EMT;

public class CalibrationLoaderComponent : MonoBehaviour
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public void Start()
    {
        var nlogConfig = Path.Combine(Application.streamingAssetsPath, "NLog.config");
        NLog.LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);

        var provider = GlobalServices.Instance.ServiceProvider.GetRequiredService<EMTConfigurationProvider>();
        var settings = provider.Configuration;

        switch (settings.Type)
        {
            case DeviceTypeEnum.DEVICE_TYPE_IPW:
                SceneManager.LoadScene("EmtToolboxScene");
                break;
            case DeviceTypeEnum.DEVICE_TYPE_PGE:
                SceneManager.LoadScene("EmtToolboxScenePge");
                break;
            case DeviceTypeEnum.DEVICE_TYPE_UNKNOWN:
            default:
                Logger.ErrorUnity($"Device type '{settings.Type}' is not supported.");
                throw new NotImplementedException($"Device type '{settings.Type}' is not supported.");
        }
    }
}
