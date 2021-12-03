using System;
using System.Collections;
using System.Collections.Generic;
using emt_sdk.Settings;
using UnityEngine;

public enum LayoutTypeEnum
{
    Grid,
    List
}

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

    public LayoutTypeEnum LayoutType;
    public List<ImageInfo> ImagePool; // No need to keep creating new images, keep a pool
    
    private GalleryLayout _layout;
    public List<Sprite> Sprites;

    void Start()
    {
        ImagePool = new List<ImageInfo>();
        
        // TODO: Debug only
        Sprites = GalleryLoader.LoadSpriteFolder("/home/ipw/Documents/gallery/");
        
        CreateLayout();
        AllocatePool();

        StartCoroutine(DelayInvalidate());
    }

    private IEnumerator DelayInvalidate()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        
        _layout.Invalidate();
    }

    void Update()
    {
        _layout.Update();
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
            LayoutTypeEnum.Grid => new GalleryGridLayout(this),
            LayoutTypeEnum.List => new GalleryListLayout(this),
            _ => throw new NotSupportedException(),
        };

        if (_layout is GalleryListLayout ll)
            ll.Orientation = 
                Camera.Orientation == IPWSetting.IPWOrientation.Horizontal
                ? GalleryListLayout.GalleryListOrientation.Horizontal
                : GalleryListLayout.GalleryListOrientation.Vertical;
    }
}
