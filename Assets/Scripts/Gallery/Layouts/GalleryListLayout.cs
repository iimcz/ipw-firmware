using UnityEngine;

public class GalleryListLayout : GalleryLayout
{
    public override int PoolSize => 5;

    public GalleryListLayout(GalleryPoolComponent pool) : base(pool) { }

    public override void Update()
    {
        if (!AutoScroll) return;

        _scrollProgress += Time.deltaTime;
        if (_scrollProgress >= ScrollDelay)
        {
            ScrollDelay -= _scrollProgress;
        }
    }
}
