using System;
using System.Collections.Generic;
using System.IO;
using Assets.Extensions;
using emt_sdk.Settings;
using Newtonsoft.Json;
using UnityEngine;

public static class ProjectorTransfomartionSettingsLoader
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static string SettingsPath
    {
        get
        {
            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var configFile = Path.Combine(userFolder, "ipw.json");
            
            return configFile;
        }
    }

    public static bool SettingsExists => File.Exists(SettingsPath);
    
    // public static IPWSetting LoadSettings()
    // {
    //     try
    //     {
    //         var json = File.ReadAllText(SettingsPath);
    //         return JsonConvert.DeserializeObject<IPWSetting>(json);
    //     }
    //     catch (Exception e)
    //     {
    //         Logger.ErrorUnity("Failed to load settings", e);
    //         return new IPWSetting
    //         {
    //             Displays = new List<DisplaySetting>
    //             {
    //                 new DisplaySetting(),
    //                 new DisplaySetting
    //                 {
    //                     DisplayId = 1
    //                 }
    //             }
    //         }; // Loading failed, assume defaults
    //     }
    // }
}