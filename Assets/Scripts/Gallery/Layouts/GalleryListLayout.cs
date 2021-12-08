using System.Collections.Generic;
using UnityEngine;

public class GalleryListLayout : GalleryLayout
{
    public enum GalleryListOrientation
    {
        Vertical,
        Horizontal
    }
    
    /// <summary>
    /// Gets or sets the amount of visible images in the list
    /// </summary>
    public int VisibleListLength { get; set; } = 6;
    
    /// <summary>
    /// Gets or sets the percentage of the screen used as spacing between each image
    /// </summary>
    public float Spacing { get; set; }

    public override int PoolSize => VisibleListLength + 2;
    public GalleryListOrientation Orientation { get; set; } = GalleryListOrientation.Horizontal;
    
    /// <summary>
    /// Index of the leftmost visible image
    /// </summary>
    private int _firstImage = 0;

    /// <summary>
    /// Index of the leftmost visible sprite
    /// </summary>
    private int _firstSprite = 0;

    private readonly List<float> _startPositions = new List<float>();
    private float SizeWithoutPadding => 1f - (2f * Padding.x);
    private float SizeWithoutSpacing => SizeWithoutPadding - ((VisibleListLength - 1) * Spacing);
    private float ImageSize => SizeWithoutSpacing / VisibleListLength;

    public GalleryListLayout(GalleryPoolComponent pool) : base(pool) { }

    public override void Update()
    {
        if (!AutoScroll) return;
        if (_startPositions.Count == 0) return; // Wait for first invalidation

        _scrollProgress += Time.deltaTime;
        if (_scrollProgress >= ScrollDelay)
        {
            _scrollProgress -= ScrollDelay;
            Previous();
        }
    }

    public override void Invalidate()
    {
        // Calculate image positions
        var pos = Padding.x;
        _startPositions.Clear();
        for (int i = 0; i < VisibleListLength; i++)
        {
            _startPositions.Add(pos);
            pos += ImageSize + Spacing;
        }
        
        var boundaries = _pool.Camera.GetBoundaries(5);
        for (int i = 0; i < PoolSize; i++)
        {
            var image = _pool.ImagePool[(_firstImage + i) % PoolSize];
            image.Position = new Vector2Int(i, 0);
            image.GameObject.transform.localScale =
                new Vector3(boundaries.width * ImageSize, boundaries.width * ImageSize, 1);

            image.GameObject.transform.localPosition = 
                i < VisibleListLength ?
                    new Vector3(boundaries.x + _startPositions[i] * boundaries.width, 0, 0) : // Onscreen
                    new Vector3(boundaries.xMax + Spacing, 0, 0); // Offsceen
            
            image.Slider.ResetPosition();
        }
    }

    public override void Next()
    {
        var boundaries = _pool.Camera.GetBoundaries(5);

        // Move first image to the left
        _pool.ImagePool[_firstImage % PoolSize].Slider.SetTarget(new Vector3(boundaries.x - (ImageSize * 2f * boundaries.width), 0, 0));
            
        // Shift remaining images
        for (int i = 1; i < VisibleListLength; i++)
        {
            _pool.ImagePool[(_firstImage + i) % PoolSize].Slider.SetTarget(new Vector3(boundaries.x + _startPositions[i - 1] * boundaries.width, 0, 0));
        }

        _firstImage++;
        _firstImage %= PoolSize;
        _firstSprite++;
        _firstSprite %= _pool.Sprites.Count;
            
        // Move in last image
        var lastImage = _pool.ImagePool[(_firstImage + VisibleListLength - 1) % PoolSize];
        lastImage.GameObject.transform.localPosition = new Vector3(boundaries.xMax + (boundaries.width * Padding.x), 0, 0);
        lastImage.Slider.SetTarget(new Vector3(boundaries.x + _startPositions[VisibleListLength - 1] * boundaries.width, 0, 0));
        lastImage.Renderer.sprite = _pool.Sprites[(_firstSprite + VisibleListLength - 1) % _pool.Sprites.Count];
    }

    public override void Previous()
    {
        var boundaries = _pool.Camera.GetBoundaries(5);
        // Move last image to the right
        _pool.ImagePool[(_firstImage + VisibleListLength - 1) % PoolSize].Slider.SetTarget(new Vector3(boundaries.x + boundaries.width + (boundaries.width * Padding.x), 0, 0));
            
        // Shift remaining images
        for (int i = VisibleListLength - 2; i >= 0; i--)
        {
            _pool.ImagePool[(_firstImage + i) % PoolSize].Slider.SetTarget(new Vector3(boundaries.x + _startPositions[i + 1] * boundaries.width, 0, 0));
        }

        _firstImage--;
        if (_firstImage < 0) _firstImage += PoolSize;
        _firstSprite--;
        if (_firstSprite < 0) _firstSprite += _pool.Sprites.Count;
            
        // Move in last image
        var lastImage = _pool.ImagePool[_firstImage];
        lastImage.GameObject.transform.localPosition = new Vector3(boundaries.x - (ImageSize * 2f * boundaries.width), 0, 0);
        lastImage.Slider.SetTarget(new Vector3(boundaries.x + _startPositions[0] * boundaries.width, 0, 0));
        lastImage.Renderer.sprite = _pool.Sprites[_firstSprite];
    }
}
