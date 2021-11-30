using System.IO;
using UnityEngine;

public class SkyboxLoader : MonoBehaviour
{
    /// <summary>
    /// Loads cubemap skybox from a file
    /// Supported formats are whatever <see cref="ImageConversion.LoadImage"/> supports <para></para>
    /// This is a blocking call and it WILL freeze the game for a second
    /// </summary>
    /// <param name="cubemapPath">Path to cubemap image</param>
    public void ApplySkybox(string cubemapPath)
    {
        var data = File.ReadAllBytes(cubemapPath);
        ApplySkybox(data);
    }

    /// <summary>
    /// Loads cubemap skybox from image data
    /// Supported formats are whatever <see cref="ImageConversion.LoadImage"/> supports <para></para>
    /// This is a blocking call and it WILL freeze the game for a second
    /// </summary>
    /// <param name="cubemapPath">Cubemap image data</param>
    public void ApplySkybox(byte[] cubemapData)
    {
        // Original size doesn't matter
        var skyboxTexture = new Texture2D(2, 2);

        ImageConversion.LoadImage(skyboxTexture, cubemapData);
        RenderSettings.skybox.mainTexture = skyboxTexture;
    }
}
