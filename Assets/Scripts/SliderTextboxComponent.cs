using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]
public class SliderTextboxComponent : MonoBehaviour
{
    public Slider Slider;

    private TMP_InputField _textbox;

    void Start()
    {
        _textbox = GetComponent<TMP_InputField>();

        Slider.onValueChanged.AddListener(new UnityAction<float>(SetValue));
        SetValue(Slider.value);
    }

    private void SetValue(float value)
    {
        if (_textbox.isFocused) return;
        _textbox.text = value.ToString(CultureInfo.InvariantCulture);
    }

    public void SetValue(string value)
    {
        if (!float.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out float num)) return;
        Slider.value = num;
    }
}
