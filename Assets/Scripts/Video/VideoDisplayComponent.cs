using System;
using emt_sdk.Scene;
using UnityEngine;
using UnityEngine.Video;

public class VideoDisplayComponent : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _videoPlayer;

    [SerializeField]
    private DualCameraComponent _camera;
    
    public void Resize(VideoScene.VideoAspectRatioEnum aspectRatio)
    {
        var viewport = _camera.GetBoundaries(5);
        var videoAspect = _videoPlayer.width / (float)_videoPlayer.height;
        
        // 10m is the default unity plane size, why it's not 1 is beyond me...
        const float planeSize = 10f;
        
        // TODO: Vertical mode
        switch (aspectRatio)
        {
            case VideoScene.VideoAspectRatioEnum.Fill:
                transform.localScale = new Vector3(viewport.width / planeSize, 1, viewport.height / planeSize);
                break;
            case VideoScene.VideoAspectRatioEnum.FitInside:
                if (videoAspect > 2) // Wider 
                {
                    transform.localScale = new Vector3(viewport.width / planeSize, 1, (viewport.width / videoAspect) / planeSize);
                }
                else // Taller
                {
                    transform.localScale = new Vector3((viewport.height * videoAspect) / planeSize, 1, viewport.height / planeSize);
                }
                break;
            case VideoScene.VideoAspectRatioEnum.FitOutside:
                throw new NotImplementedException();
            default:
                throw new NotSupportedException();
        }
        
        transform.localPosition = new Vector3(viewport.center.x, viewport.center.y, 5);
    }
}
