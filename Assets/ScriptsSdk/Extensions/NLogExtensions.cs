using System;
using UnityEngine;

namespace Assets.Extensions
{
    // Instead of catching unity logs and passing them to nlog it's better to do this
    // We would use class name info in nlog otherwise
    public static class NLogExtensions
    {
        public static void DebugUnity(this NLog.Logger logger, string message)
        {
            logger.Debug(message);
            Debug.Log(message);
        }

        public static void InfoUnity(this NLog.Logger logger, string message)
        {
            logger.Info(message);
            Debug.Log(message);
        }

        public static void ErrorUnity(this NLog.Logger logger, string message, Exception e)
        {
            logger.Error(e, message);
            Debug.LogError(message + e);
        }

        public static void ErrorUnity(this NLog.Logger logger, string message)
        {
            logger.Error(message);
            Debug.LogError(message);
        }
    }
}
