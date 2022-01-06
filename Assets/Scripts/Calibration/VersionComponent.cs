using TMPro;
using UnityEngine;

public class VersionComponent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        _text.text = $"IPW v{Application.version}" + _text.text;
    }
}
