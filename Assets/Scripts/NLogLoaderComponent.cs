using Assets.Extensions;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Threading.Tasks;
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
            // Load XML from StreamingAssets
            var nlogConfig = Path.Combine(Application.streamingAssetsPath, _configName);
            LogManager.Configuration = new XmlLoggingConfiguration(nlogConfig);
            Logger.InfoUnity($"Loaded NLog config from '{nlogConfig}'");
        }

        public static void LogUnity(string level, string message)
        {
            switch (level)
            {
                case "Warn":
                    Debug.LogWarning(message);
                    break;
                case "Error":
                    Debug.LogError(message);
                    break;
                default:
                    print($"{level} | {message}");;
                    break;
            }
        }
    }
}
