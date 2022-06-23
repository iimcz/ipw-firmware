using TMPro;
using UnityEngine;

public class VersionComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _deviceName;

    private void Start()
    {
        _text.text = $"{_deviceName} v{Application.version}" + _text.text;
    }
}
