public abstract class GalleryLayout
{
    protected GalleryPoolComponent _pool;
    protected float _scrollProgress;

    public bool AutoScroll { get; set; }
    public float ScrollDelay { get; set; }

    public float Padding { get; set; } // Just a random unity measurement, needs to be redone (pixels?)

    /// <summary>
    /// Recommended pool size to ensure working animations
    /// </summary>
    public virtual int PoolSize { get; }

    public GalleryLayout(GalleryPoolComponent pool)
    {
        _pool = pool;
    }

    public abstract void Update();
    public abstract void Invalidate();
    public abstract void Next();
    public abstract void Previous();
}
