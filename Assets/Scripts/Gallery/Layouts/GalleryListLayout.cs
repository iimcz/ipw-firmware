using System.Collections.Generic;
using Microsoft.CodeAnalysis.Scripting.Hosting;
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
    public int VisibleListLength { get; set; } = 2;
    
    /// <summary>
    /// Gets or sets the percentage of the screen used as padding on the border of the gallery
    /// </summary>
    public Vector2 Padding { get; set; }
    
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

    private List<float> _startPositions = new List<float>();
    private float SizeWithoutPadding => 1f - (2f * Padding.x);
    private float SizeWithoutSpacing => SizeWithoutPadding - ((VisibleListLength - 1) * Spacing);
    private float ImageSize => SizeWithoutSpacing / VisibleListLength;

    public GalleryListLayout(GalleryPoolComponent pool) : base(pool) { }

    public override void Update()
    {
        //if (!AutoScroll) return;

        if (_startPositions.Count == 0) return; // Wait for first invalidation

        _scrollProgress += Time.deltaTime;
        if (_scrollProgress >= ScrollDelay)
        {
            var boundaries = _pool.Camera.GetBoundaries(5);
            _scrollProgress -= ScrollDelay;

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
            lastImage.GameObject.transform.localPosition = new Vector3(boundaries.x + boundaries.width + (boundaries.width * Padding.x), 0, 0);
            lastImage.Slider.SetTarget(new Vector3(boundaries.x + _startPositions[VisibleListLength - 1] * boundaries.width, 0, 0));
            lastImage.Renderer.sprite = _pool.Sprites[(_firstSprite + VisibleListLength - 1) % _pool.Sprites.Count];
        }
    }

    public override void Invalidate()
    {
        Padding = new Vector2(0.05f, 0.1f);
        Spacing = 0.02f;
        ScrollDelay = 2f;
        
        var pos = Padding.x;
        for (int i = 0; i < VisibleListLength; i++)
        {
            _startPositions.Add(pos);
            pos += ImageSize + Spacing;
        }
        
        var boundaries = _pool.Camera.GetBoundaries(5);

        int _visibleImages = 0;
        
        for (int i = 0; i < PoolSize; i++)
        {
            var image = _pool.ImagePool[(_firstImage + i) % PoolSize];
            image.Position = new Vector2Int(i, 0);
            image.GameObject.transform.localScale =
                new Vector3(boundaries.width * ImageSize, boundaries.width * ImageSize, 1);

            if (i < VisibleListLength) // Is visible
            {
                image.GameObject.transform.localPosition = new Vector3(boundaries.x + _startPositions[i] * boundaries.width, 0, 0);
            }
            else
            {
                image.GameObject.transform.localPosition = new Vector3(boundaries.x + boundaries.width + Spacing, 0, 0);
            }
            
            image.Slider.ResetPosition();
        }
    }
}
