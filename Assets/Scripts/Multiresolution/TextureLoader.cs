using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public static class TextureLoader
{
    //public const string CONTENT_ROOT = "G:/temp/Endless Night/resource/";
    public static string CONTENT_ROOT = Application.streamingAssetsPath;

    private const int BUFFER_SIZE = 30000000;
    private static byte[] _header = new byte[148];

    public static Texture2D LoadTexture(string path, int mip = 0)
    {
        var file = File.OpenRead(path);
        file.Read(_header, 0, 148);

        int height = _header[13] * 256 + _header[12];
        int width = _header[17] * 256 + _header[16];

        var skip = 0;
        while (mip > 0)
        {
            skip += GetMipLength(width, height);
            width /= 2;
            height /= 2;
            mip--;
        }

        if (skip > 0) file.Seek(skip, SeekOrigin.Current);

        var texture = new Texture2D(width, height, TextureFormat.BC7, mip == -1);
        unsafe
        {
            var textureData = texture.GetRawTextureData<byte>();
            byte* ptr = (byte*)textureData.GetUnsafePtr();
            var ustream = new UnmanagedMemoryStream(ptr, BUFFER_SIZE, BUFFER_SIZE, FileAccess.Write);

            if (mip == -1) file.CopyTo(ustream);
            else file.CopyTo(ustream, GetMipLength(width, height));
        }

        texture.Apply(false, true);
        texture.wrapMode = TextureWrapMode.Clamp;
        file.Close();

        return texture;
    }

    private static int GetMipLength(int width, int height)
    {
        return Mathf.Max(1, (width + 3) / 4) * Mathf.Max(1, (height + 3) / 4) * 16;
    }
    }