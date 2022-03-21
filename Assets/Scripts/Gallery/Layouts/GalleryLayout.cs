using Naki3D.Common.Protocol;

public abstract class GalleryLayout
{
    protected GalleryPoolComponent _pool;
    protected float _scrollProgress;

    public bool AutoScroll => ScrollDelay != 0f;
    public float ScrollDelay { get; set; }

    /// <summary>
    /// Gets or sets the percentage of the screen used as padding on the border of the gallery
    /// </summary>
    public UnityEngine.Vector2 Padding { get; set; }

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

    public abstract void Gesture(GestureData gesture);
}
