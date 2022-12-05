using Naki3D.Common.Protocol;
using UnityEngine;

public class MultiresolutionCamera : MonoBehaviour
{
    [SerializeField]
    private CameraRigSpawnerComponent _rigSpawner;

    public int Layer;

    /// <summary>
    /// Determines how many extra tiles will be loaded in every direction
    /// </summary>
    [Min(1)]
    public int TilePadding;

    /// <summary>
    /// Determines how many tiles will be visible horizontally. Vertical FOV is determined automatically.
    /// </summary>
    public int HorizontalTileCount;
    public int VerticalTileCount { get; private set; }

    private float _tileSize;

    void Start()
    {
        // Determine how many tiles we'll show
        var screenBoundaries = _rigSpawner.CameraRig.GetBoundaries(1);
        _tileSize = screenBoundaries.width / HorizontalTileCount;
        VerticalTileCount = (int)Mathf.Ceil(screenBoundaries.height / _tileSize);
    }

    void Update()
    {
        // TODO: Debug, remove
    }

    public void OnEvent(SensorMessage message)
    {

    }
}
