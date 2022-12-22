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

    public MeshRenderer VideoRenderer;
    private Material _videoMaterial;
    private RenderTexture _renderTexture;

    public void Start()
    {
        _videoMaterial = VideoRenderer.material;
    }

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
        // 10m is the default unity plane size, why it's not 1 is beyond me...
        const float planeSize = 10f;

        var cameraViewport = RigSpawner.CameraRig.GetBoundaries(5);
        var canvasSize = RigSpawner.CameraRig.CanvasDimensions;

        // Match viewport and video size
        transform.localScale = new Vector3(cameraViewport.width / planeSize, 1, cameraViewport.height / planeSize);

        var viewportScale = new Vector3(
            RigSpawner.CameraRig.CanvasDimensions.x / RigSpawner.CameraRig.Viewport.Width,
            1,
            RigSpawner.CameraRig.CanvasDimensions.y / RigSpawner.CameraRig.Viewport.Height
        );

        var scale = transform.localScale;
        scale.Scale(viewportScale);
        transform.localScale = scale;

        _renderTexture = new RenderTexture((int)canvasSize.x, (int)canvasSize.y, 0);
        _videoMaterial.mainTexture = _renderTexture;

        VideoPlayer.targetTexture = _renderTexture;
        VideoPlayer.aspectRatio = aspectRatio switch
        {
            VideoScene.VideoAspectRatioEnum.Stretch => VideoAspectRatio.Stretch,
            VideoScene.VideoAspectRatioEnum.FitInside => VideoAspectRatio.FitInside,
            VideoScene.VideoAspectRatioEnum.FitOutside => VideoAspectRatio.FitOutside,
            _ => throw new NotSupportedException(),
        };


        // Left align video
        var displayedSize = new Vector2(RigSpawner.CameraRig.Viewport.Width, RigSpawner.CameraRig.Viewport.Height);
        var offsetFromCenter = (canvasSize - displayedSize) / 2f;
        var pixelShift = new Vector2(RigSpawner.CameraRig.Viewport.X, RigSpawner.CameraRig.Viewport.Y) - offsetFromCenter;
        pixelShift.Scale(new Vector2(1f / displayedSize.x, 1f / displayedSize.y));

        RigSpawner.CameraRig.SyncLensShift = pixelShift;
    }
}
