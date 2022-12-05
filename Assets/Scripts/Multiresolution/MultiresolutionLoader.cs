using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiresolutionLoader : MonoBehaviour
{
    // Tile resolution in pixels
    public const int TILE_RESOLUTION = 256;

    // Tile size in kB (roughly)
    public const int TILE_SIZE = 65;

    [Range(0f, 1f)]
    public float GpuMemoryAllocation = 0.75f;

    private int _availableTiles;

    [SerializeField]
    private GameObject _markerPrefab;
    private List<MarkerComponent> _markers = new();

    IEnumerator Start()
    {
        // Wait two frames for the camera transformation to apply
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Determine how many tiles we can load
        var allocatedMemory = SystemInfo.graphicsMemorySize * GpuMemoryAllocation;
        _availableTiles = (int)(allocatedMemory * 1024 / TILE_SIZE);

        // Debug mode detection
        if (ExhibitConnectionComponent.ActivePackage == null) SpawnDebugScene();
        else SpawnLoadedScene();
    }

    public Texture2D RequestTile(int x, int y)
    {
        return null;
    }

    
    private void SpawnDebugScene()
    {

    }

    private void SpawnLoadedScene()
    {
        // TODO: JSON
    }

    private void SpawnMarker(Texture2D texture, MarkerInfo info)
    {
        var marker = Instantiate(_markerPrefab);
        var markerComponent = marker.GetComponent<MarkerComponent>();

        _markers.Add(markerComponent);
        markerComponent.Apply(texture, info);
    }
}
