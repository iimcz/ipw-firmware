using UnityEngine;

public class GalleryGridLayout : GalleryLayout
{
    public int Rows { get; set; }
    public int Columns { get; set; }

    public override int PoolSize => Rows * Columns + (Columns * 2);

    public GalleryGridLayout(GalleryPoolComponent pool) : base(pool) { }

    public override void Update()
    {
        if (!AutoScroll) return;

        _scrollProgress += Time.deltaTime;
        if (_scrollProgress >= ScrollDelay)
        {
            ScrollDelay -= _scrollProgress;
        }
    }

    public override void Invalidate()
    {
    }
}