using System;
using System.Collections;
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
        // TODO: Wait until transform applies!!!!
        StartCoroutine(DelayApply());
    }

    private IEnumerator DelayApply()
    {
        yield return new WaitForSecondsRealtime(2f);
        _display.Resize(VideoScene.VideoAspectRatioEnum.FitInside);
    }

    public void LoadVideo(string path)
    {
        _player.url = $"file://{path}";
        _player.Play();
        
        _display.Resize(VideoScene.VideoAspectRatioEnum.Fill);
    }
}
