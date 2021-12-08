using System;
using System.Collections;
using System.IO;
using emt_sdk.Scene;
using UnityEngine;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _player;

    [SerializeField] 
    private VideoDisplayComponent _display;

    public void Start()
    {
        StartCoroutine(DelayApply());
    }

    public void Apply(VideoScene scene, string basePath)
    {
        if (ColorUtility.TryParseHtmlString(scene.BackgroundColor, out var backgroundColor) == false)
            throw new ArgumentException("Background color is not a valid HTML hex color string",
                nameof(scene.BackgroundColor));

        _display.Camera.TopCamera.Camera.backgroundColor = backgroundColor;
        _display.Camera.BottomCamera.Camera.backgroundColor = backgroundColor;
        
        var fileName = Path.Combine(basePath, scene.FileName);
        _player.url = $"file://{fileName}";
        _player.isLooping = scene.Loop;
        
        // Make sure we get the correct info for resizing
        _player.Prepare();
        
        if (scene.AutoStart) _player.Play();
        else _player.Stop();
        
        _display.Resize(scene.AspectRatio);
    }

    private IEnumerator DelayApply()
    {
        // Wait two frames for the camera transformation to apply
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        // TODO: Store a copy of the received data in some static manager and use it here
        //Apply();
    }
}
