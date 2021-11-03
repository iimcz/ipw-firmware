using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DisplayIndexIdentifierComponent : MonoBehaviour
{
    [Required]
    [SceneObjectsOnly]
    public Camera Camera;

    [Required]
    [SceneObjectsOnly]
    public TextMeshProUGUI Text;

    private int _displayId;
    
    void Start()
    {
        _displayId = Camera.targetDisplay;
    }
    
    void Update()
    {
        if (_displayId != Camera.targetDisplay) Text.text = _displayId.ToString();
    }
}
