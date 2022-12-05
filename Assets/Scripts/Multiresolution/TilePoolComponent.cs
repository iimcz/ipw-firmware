using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TilePoolComponent : MonoBehaviour
{
    public class TileInfo
    {
        public GameObject GameObject { get; set; }
        public Material Material { get; set; }
    }

    [SerializeField]
    private GameObject _tilePrefab;

    private readonly ObjectPool<TileInfo> _tilePool;

    public void Allocate(int tiles)
    {/*
        if (_tiles.Count > 0) throw new InvalidOperationException("Pool has already been allocated once");

        for (int i = 0; i < tiles; i++)
        {
            var tile = Instantiate(_tilePrefab, transform);

            _tiles.Add(new TileInfo
            {
                GameObject = tile,
                Material = tile.GetComponent<MeshRenderer>().material
            });

            // Hide tile for future use
            tile.transform.localPosition = Vector3.one * 10000;
        }
        */
    }
}
