using System;
using System.Collections.Generic;
using System.IO;
using emt_sdk.Settings;
using Newtonsoft.Json;
using UnityEngine;

public static class ProjectorTransfomartionSettingsLoader
{
    public static IPWSetting LoadSettings()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFile = Path.Combine(userFolder, "ipw.json");
        try
        {
            var json = File.ReadAllText(configFile);
            return JsonConvert.DeserializeObject<IPWSetting>(json);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return new IPWSetting
            {
                Displays = new List<DisplaySetting>
                {
                    new DisplaySetting(),
                    new DisplaySetting()
                }
            }; // Loading failed, assume defaults
        }
    }
}