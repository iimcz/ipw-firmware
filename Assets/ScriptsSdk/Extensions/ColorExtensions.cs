using emt_sdk.Settings;
using UnityEngine;

public static class ColorExtensions
{
    public static Color ToUnityColor(this ColorSetting.Color color)
    {
        return new Color(color.R, color.G, color.B);
    }
}