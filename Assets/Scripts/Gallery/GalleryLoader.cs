using System.IO;
using UnityEngine;

public static class GalleryLoader
{
    public static Texture2D LoadTexture(string path)
    {
        // We could reduce copying by loading straight into the texture, but this only gets done once at startup
        var data = File.ReadAllBytes(path);
        var texture = new Texture2D(2, 2);
        if (!texture.LoadImage(data, false)) throw new InvalidDataException("Image is not in a supported format");
        return texture;
    }

    public static Sprite LoadSprite(string path)
    {
        var texture = LoadTexture(path);
        var rect = new Rect(0, 0, texture.width, texture.height);
        return Sprite.Create(texture, rect, Vector2.zero);
    }
}
