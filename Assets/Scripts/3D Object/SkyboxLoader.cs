using System.IO;
using UnityEngine;

public static class SkyboxLoader
{
    /// <summary>
    /// Loads cubemap skybox from a file
    /// Supported formats are whatever <see cref="ImageConversion.LoadImage"/> supports <para></para>
    /// This is a blocking call and it WILL freeze the game for a second
    /// </summary>
    /// <param name="cubemapPath">Path to cubemap image</param>
    public static void ApplySkybox(string cubemapPath, Color tint)
    {
        var data = File.ReadAllBytes(cubemapPath);
        ApplySkybox(data, tint);
    }

    /// <summary>
    /// Loads cubemap skybox from image data
    /// Supported formats are whatever <see cref="ImageConversion.LoadImage"/> supports <para></para>
    /// This is a blocking call and it WILL freeze the game for a second
    /// </summary>
    /// <param name="cubemapPath">Cubemap image data</param>
    public static void ApplySkybox(byte[] cubemapData, Color tint)
    {
        // Original size doesn't matter
        var skyboxTexture = new Texture2D(2, 2);

        ImageConversion.LoadImage(skyboxTexture, cubemapData);
        RenderSettings.skybox.SetTexture("_Tex", skyboxTexture);
        RenderSettings.skybox.SetColor("_Tint", tint);
    }

    /// <summary>
    /// Uses a solid color for the skybox.
    /// </summary>
    /// <param name="tint">Color of the skybox</param>
    public static void ApplyTint(Color tint)
    {
        RenderSettings.skybox.SetColor("_Tint", tint);
    }
}
