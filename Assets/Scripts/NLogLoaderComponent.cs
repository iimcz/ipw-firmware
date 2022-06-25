using Assets.Extensions;
using NLog.Config;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class NLogLoaderComponent : MonoBehaviour
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [SerializeField]
        private string _configName = "NLog.config";

        void Start()
        {
            var nlogConfig = Path.Combine(Application.streamingAssetsPath, _configName);
            NLog.LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);

            Logger.InfoUnity($"Loaded NLog config from '{nlogConfig}'");
        }
    }
}
