using System;
using emt_sdk.Scene;
using emt_sdk.Settings;
using UnityEngine;
using UnityEngine.Video;
using Vector3 = UnityEngine.Vector3;

public class VideoDisplayComponent : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public CameraRigSpawnerComponent RigSpawner;

    public void SetVolume(double volume)
    {
        // Only works for direct, single track audio
        VideoPlayer.SetDirectAudioVolume(0, (float)volume);
    }

    public void Play()
    {
        if (!VideoPlayer.isPlaying) VideoPlayer.Play();
    }
    
    public void Resize(VideoScene.VideoAspectRatioEnum aspectRatio)
    {
        var viewport = RigSpawner.CameraRig.GetBoundaries(5);
        var videoAspect = VideoPlayer.width / (float)VideoPlayer.height;

        // 10m is the default unity plane size, why it's not 1 is beyond me...
        const float planeSize = 10f;

        var widthFit = new Vector3(viewport.width / planeSize, 1, (viewport.width / videoAspect) / planeSize);
        var heightFit = new Vector3((viewport.height * videoAspect) / planeSize, 1, viewport.height / planeSize);

        if (RigSpawner.CameraRig.Orientation == IPWSetting.IPWOrientation.Horizontal)
        {
            switch (aspectRatio)
            {
                case VideoScene.VideoAspectRatioEnum.Stretch:
                    transform.localScale = new Vector3(viewport.width / planeSize, 1, viewport.height / planeSize);
                    break;
                case VideoScene.VideoAspectRatioEnum.FitInside:
                    transform.localScale = videoAspect > 2 ? widthFit : heightFit;
                    break;
                case VideoScene.VideoAspectRatioEnum.FitOutside:
                    transform.localScale = videoAspect > 2 ? heightFit : widthFit;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        else
        {
            switch (aspectRatio)
            {
                case VideoScene.VideoAspectRatioEnum.Stretch:
                    transform.localScale = new Vector3(viewport.width / planeSize, 1, viewport.height / planeSize);
                    break;
                case VideoScene.VideoAspectRatioEnum.FitInside:
                    transform.localScale = videoAspect < 2 ? widthFit : heightFit;
                    break;
                case VideoScene.VideoAspectRatioEnum.FitOutside:
                    transform.localScale = videoAspect < 2 ? heightFit : widthFit;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        transform.localPosition = new Vector3(viewport.center.x, viewport.center.y, 5);
    }
}
