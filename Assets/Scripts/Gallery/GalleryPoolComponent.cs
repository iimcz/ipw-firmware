using System;
using System.Collections;
using System.Collections.Generic;
using Assets.ScriptsSdk.Extensions;
using emt_sdk.Scene;
using emt_sdk.Settings;
using Naki3D.Common.Protocol;
using UnityEngine;

public class ImageInfo
{
    public Vector2Int Position;
    public SpriteRenderer Renderer;
    public GameObject GameObject;
    public SlideComponent Slider;
}

public class GalleryPoolComponent : MonoBehaviour
{
    public GameObject ImagePrefab;
    public DualCameraComponent Camera;

    public Gallery.GalleryLayoutEnum LayoutType;
    public List<ImageInfo> ImagePool; // No need to keep creating new images, keep a pool
    
    private GalleryLayout _layout;
    public List<Sprite> Sprites;

    void Start()
    {
        ImagePool = new List<ImageInfo>();
        StartCoroutine(DelayApply());
    }

    public void Apply(Gallery scene, string basePath)
    {
        if (ColorUtility.TryParseHtmlString(scene.BackgroundColor, out var backgroundColor) == false)
            throw new ArgumentException("Background color is not a valid HTML hex color string", nameof(scene.BackgroundColor));
        
        Camera.TopCamera.Camera.backgroundColor = backgroundColor;
        Camera.BottomCamera.Camera.backgroundColor = backgroundColor;

        LayoutType = scene.LayoutType;
        CreateLayout();
        
        switch (scene.Layout)
        {
            case Gallery.ListLayout list:
                var listLayout = (GalleryListLayout) _layout;
                listLayout.Padding = scene.Padding.ToUnityVector();
                listLayout.Spacing = list.Spacing;
                listLayout.VisibleListLength = list.VisibleImages;
                listLayout.ScrollDelay = scene.ScrollDelay;

                // TODO: Debug only
                Sprites = GalleryLoader.LoadSpriteFolder("/home/ipw/Documents/gallery/");
                /*
                Sprites = list.Images.Select(i =>
                {
                    var fileName = Path.Combine(basePath, i.FileName);
                    return GalleryLoader.LoadSprite(fileName);
                }).ToList();
                */
                break;
            case Gallery.GridLayout grid:
                break;
        }
        
        AllocatePool();
        _layout.Invalidate();
    }

    private IEnumerator DelayApply()
    {
        // Wait two frames for the camera transformation to apply
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        // TODO: Store a copy of the received data in some static manager and use it here
       Apply(new Gallery
       {
           BackgroundColor = "#000000",
           LayoutType = Gallery.GalleryLayoutEnum.List,
           Layout = new Gallery.ListLayout
           {
               Spacing = 0.1f,
               VisibleImages = 2
           },
           Padding = new Naki3D.Common.Protocol.Vector2
           {
               X = 0.1f,
               Y = 0.1f
           },
           ScrollDelay = 0f,
           SlideAnimationLength = 0.3f
       }, string.Empty);
    }

    void Update()
    {
        // Wait until layout is properly created
        if (_layout != null) _layout.Update();
    }

    private void AllocatePool()
    {
        foreach (var image in ImagePool) Destroy(image.GameObject);

        var poolSize = _layout.PoolSize;
        for (int i = 0; i < poolSize; i++)
        {
            var image = Instantiate(ImagePrefab, transform);
            var imageInfo = new ImageInfo
            {
                GameObject = image,
                Renderer = image.GetComponent<SpriteRenderer>(),
                Slider = image.GetComponent<SlideComponent>(),
                Position = Vector2Int.zero
            };

            imageInfo.Renderer.sprite = Sprites[i % Sprites.Count];
            ImagePool.Add(imageInfo);
        }
    }

    public void CreateLayout()
    {
        _layout = LayoutType switch
        {
            Gallery.GalleryLayoutEnum.Grid => new GalleryGridLayout(this),
            Gallery.GalleryLayoutEnum.List => new GalleryListLayout(this),
            _ => throw new NotSupportedException(),
        };

        if (_layout is GalleryListLayout ll)
            ll.Orientation = 
                Camera.Orientation == IPWSetting.IPWOrientation.Horizontal
                ? GalleryListLayout.GalleryListOrientation.Horizontal
                : GalleryListLayout.GalleryListOrientation.Vertical;
    }

    public void OnEvent(SensorMessage e)
    {
        if (e.DataCase == SensorMessage.DataOneofCase.Gesture) _layout.Gesture(e.Gesture);
        else if (e.DataCase == SensorMessage.DataOneofCase.KeyboardUpdate)
        {
            if (e.KeyboardUpdate.Type == KeyActionType.KeyUp) return;

            if (e.KeyboardUpdate.Keycode == (int)KeyCode.LeftArrow) _layout.Previous();
            else if (e.KeyboardUpdate.Keycode == (int)KeyCode.RightArrow) _layout.Next();
        }
    }
}
