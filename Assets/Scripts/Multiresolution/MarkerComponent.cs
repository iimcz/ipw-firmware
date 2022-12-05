using TMPro;
using UnityEngine;

public class MarkerComponent : MonoBehaviour
{
    private MarkerInfo _currentInfo;

    private Material _material;

    [SerializeField]
    private TextMeshProUGUI _text;

    void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    public void Apply(Texture2D texture, MarkerInfo info)
    {
        _material.mainTexture = texture;
        _text.text = info.Title;
    }

    public void OnLayerChanged(int layer)
    {
        // Disable / enable based on zoom distance
        var layerDelta = Mathf.Abs(layer - _currentInfo.ZoomLevel);
        gameObject.SetActive(layerDelta <= _currentInfo.ZoomRange);
    }
}
