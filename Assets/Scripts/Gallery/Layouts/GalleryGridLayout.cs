using Naki3D.Common.Protocol;
using UnityEngine;
using System.Collections.Generic;

public class GalleryGridLayout : GalleryLayout
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public UnityEngine.Vector2 Spacing { get; set; }

    public override int PoolSize => Rows * Columns + Rows;


    private readonly List<UnityEngine.Vector2> _startPositions = new List<UnityEngine.Vector2>();
    private UnityEngine.Vector2 SizeWithoutPadding => new UnityEngine.Vector2(1f, 1f) - (2f * Padding);
    private UnityEngine.Vector2 SizeWithoutSpacing => SizeWithoutPadding - (new UnityEngine.Vector2(Columns, Rows) * Spacing);
    private UnityEngine.Vector2 ImageSize => SizeWithoutSpacing / new UnityEngine.Vector2(Columns, Rows);
    private UnityEngine.Vector2 HalfSlotSize => new UnityEngine.Vector2(1f, 1f) / new UnityEngine.Vector2(Columns, Rows) / 2.0f;

    private int _firstImage = 0;

    private int _firstSprite = 0;

    public GalleryGridLayout(GalleryPoolComponent pool) : base(pool) { }

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
        var pos = new UnityEngine.Vector2(Padding.x, Padding.y);
        _startPositions.Clear();
        for (int i = 0; i < Columns; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                _startPositions.Add(pos);
                pos.y += ImageSize.y + Spacing.y;
            }
            pos.y = Padding.y;
            pos.x += ImageSize.x + Spacing.x;
        }
        
        var boundaries = _pool.RigSpawner.CameraRig.GetBoundaries(5);
        for (int i = 0; i < PoolSize; i++)
        {
            var image = _pool.ImagePool[(_firstImage + i) % PoolSize];
            image.Position = new Vector2Int(i / Rows, i % Rows);
            image.GameObject.transform.localScale =
                new UnityEngine.Vector3(boundaries.width * ImageSize.x, boundaries.width * ImageSize.y, 1);

            int row = i % Rows;
            int col = i / Rows;

            image.GameObject.transform.localPosition = 
                col < Columns ?
                    new UnityEngine.Vector3(boundaries.x + _startPositions[i].x * boundaries.width, boundaries.yMax - (_startPositions[i].y + HalfSlotSize.y) * boundaries.height, 0) : // Onscreen
                    new UnityEngine.Vector3(boundaries.xMax + Spacing.x, boundaries.yMax - (_startPositions[row].y + HalfSlotSize.y) * boundaries.height, 0); // Offsceen
            
            image.Slider.ResetPosition();
        }
    }

    public override void Next()
    {
        var boundaries = _pool.RigSpawner.CameraRig.GetBoundaries(5);

        // Move the first column to the left
        for (int i = 0; i < Rows; i++)
        {
            var image = _pool.ImagePool[(_firstImage + i) % PoolSize];
            var target = image.GameObject.transform.localPosition;
            target.x = boundaries.x - (ImageSize.x * 2f * boundaries.width);
            image.Slider.SetTarget(target);
        }

        // Shift remaining images
        for (int i = Rows; i < Rows * Columns; i++)
        {
            var pos = i - Rows;
            _pool.ImagePool[(_firstImage + i) % PoolSize].Slider.SetTarget(new UnityEngine.Vector3(boundaries.x + _startPositions[pos].x * boundaries.width, boundaries.yMax - (_startPositions[pos].y + HalfSlotSize.y) * boundaries.height, 0));
        }

        _firstImage += Rows;
        _firstImage %= PoolSize;
        _firstSprite += Rows;
        _firstSprite %= _pool.Sprites.Count;

        // Move in last column
        for (int i = 0; i < Rows; i++)
        {
            var lastImage = _pool.ImagePool[(_firstImage + (Columns - 1) * Rows + i) % PoolSize];
            lastImage.GameObject.transform.localPosition = new UnityEngine.Vector3(boundaries.xMax + Spacing.x, boundaries.yMax - (_startPositions[i].y + HalfSlotSize.y) * boundaries.height, 0);
            lastImage.Slider.SetTarget(new UnityEngine.Vector3(boundaries.x + _startPositions[(Columns - 1) * Rows + i].x * boundaries.width, boundaries.yMax - (_startPositions[(Columns - 1) * Rows + i].y + HalfSlotSize.y) * boundaries.height, 0));
            lastImage.Renderer.sprite = _pool.Sprites[(_firstImage + (Columns - 1) * Rows + i) % _pool.Sprites.Count];
        }
    }

    public override void Previous()
    {
        var boundaries = _pool.RigSpawner.CameraRig.GetBoundaries(5);

        // Move the last column to the right
        for (int i = 0; i < Rows; i++)
        {
            var image = _pool.ImagePool[(_firstImage + (Columns - 1) * Rows + i) % PoolSize];
            var target = image.GameObject.transform.localPosition;
            target.x = boundaries.xMax + Spacing.x;
            image.Slider.SetTarget(target);
        }

        // Shift remaining images
        for (int i = 0; i < Rows * (Columns - 1); i++)
        {
            var pos = i + Rows;
            _pool.ImagePool[(_firstImage + i) % PoolSize].Slider.SetTarget(new UnityEngine.Vector3(boundaries.x + _startPositions[pos].x * boundaries.width, boundaries.yMax - (_startPositions[pos].y + HalfSlotSize.y) * boundaries.height, 0));
        }

        _firstImage += PoolSize - Rows;
        _firstImage %= PoolSize;
        _firstSprite += PoolSize - Rows;
        _firstSprite %= _pool.Sprites.Count;

        // Move in last column
        for (int i = 0; i < Rows; i++)
        {
            var lastImage = _pool.ImagePool[(_firstImage + i) % PoolSize];
            lastImage.GameObject.transform.localPosition = new UnityEngine.Vector3(boundaries.x - (ImageSize.x * 2f * boundaries.width), boundaries.yMax - (_startPositions[i].y + HalfSlotSize.y) * boundaries.height, 0);
            lastImage.Slider.SetTarget(new UnityEngine.Vector3(boundaries.x + _startPositions[i].x * boundaries.width, boundaries.yMax - (_startPositions[i].y + HalfSlotSize.y) * boundaries.height, 0));
            lastImage.Renderer.sprite = _pool.Sprites[(_firstImage + i) % _pool.Sprites.Count];
        }
    }

    public override void Gesture(GestureData gesture)
    {
        // Ignore input in automatic scrolling mode
        if (AutoScroll) return;

        switch (gesture.Type)
        {
            case Naki3D.Common.Protocol.HandGestureType.GestureSwipeLeft:
                Next();
                break;
            case Naki3D.Common.Protocol.HandGestureType.GestureSwipeRight:
                Previous();
                break;
        };
    }
}