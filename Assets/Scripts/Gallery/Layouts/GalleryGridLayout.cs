using Naki3D.Common.Protocol;
using UnityEngine;
using System.Collections.Generic;

public class GalleryGridLayout : GalleryLayout
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public UnityEngine.Vector2 Spacing { get; set; }

    public override int PoolSize => Rows * Columns + (Columns * 2);


    private readonly List<UnityEngine.Vector2> _startPositions = new List<UnityEngine.Vector2>();
    private UnityEngine.Vector2 SizeWithoutPadding => new UnityEngine.Vector2(1f, 1f) - (2f * Padding);
    private UnityEngine.Vector2 SizeWithoutSpacing => SizeWithoutPadding - ((/*VisibleListLength*/0 - 1) * Spacing);
    private UnityEngine.Vector2 ImageSize => SizeWithoutSpacing / /*VisibleListLength;*/1;

    private int _firstImage = 0;

    private int _firstSprite = 0;

    public GalleryGridLayout(GalleryPoolComponent pool) : base(pool) { }

    public override void Update()
    {
        if (!AutoScroll) return;

        _scrollProgress += Time.deltaTime;
        if (_scrollProgress >= ScrollDelay)
        {
            _scrollProgress -= ScrollDelay;
        }
    }

    public override void Invalidate()
    {
        // Calculate image positions
        var pos = new UnityEngine.Vector2(Padding.x, Padding.y);
        _startPositions.Clear();
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                _startPositions.Add(pos);
                pos.y += ImageSize.y + Spacing.y;
            }
            pos.x += ImageSize.x + Spacing.x;
        }
        
        var boundaries = _pool.Camera.GetBoundaries(5);
        for (int i = 0; i < PoolSize; i++)
        {
            var image = _pool.ImagePool[(_firstImage + i) % PoolSize];
            image.Position = new Vector2Int(i / Rows, i % Rows);
            image.GameObject.transform.localScale =
                new UnityEngine.Vector3(boundaries.width * ImageSize.x, boundaries.width * ImageSize.y, 1);

            //image.GameObject.transform.localPosition = 
            //    i < VisibleListLength ?
            //        new UnityEngine.Vector3(boundaries.x + _startPositions[i] * boundaries.width, 0, 0) : // Onscreen
            //        new UnityEngine.Vector3(boundaries.xMax + Spacing, 0, 0); // Offsceen
            
            image.Slider.ResetPosition();
        }
    }

    public override void Next()
    {
        throw new System.NotImplementedException();
    }

    public override void Previous()
    {
        throw new System.NotImplementedException();
    }

    public override void Gesture(GestureData gesture)
    {
        throw new System.NotImplementedException();
    }
}