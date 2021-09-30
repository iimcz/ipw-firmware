using System;
using System.Collections.Generic;
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
    private GalleryLayout _layout;

    private Queue<ImageInfo> _imagePool; // No need to keep creating new images, keep a pool

    void Start()
    {
        _imagePool = new Queue<ImageInfo>();

        AllocatePool();
        CreateLayout();
    }

    void Update()
    {
        _layout.Update();
    }

    private void AllocatePool()
    {
        foreach (var image in _imagePool) Destroy(image.GameObject);

        var poolSize = _layout.PoolSize;
        for (int i = 0; i < poolSize; i++)
        {
            var image = Instantiate(ImagePrefab, transform);
            image.SetActive(false);

            var imageInfo = new ImageInfo
            {
                GameObject = image,
                Renderer = image.GetComponent<SpriteRenderer>(),
                Slider = image.GetComponent<SlideComponent>(),
                Position = Vector2Int.zero
            };

            _imagePool.Enqueue(imageInfo);
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
    }
}
