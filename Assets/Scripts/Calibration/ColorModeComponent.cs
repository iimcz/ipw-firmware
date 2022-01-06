using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorModeComponent : MonoBehaviour
{
    [SerializeField] private ColorChangeComponent _color;
    [SerializeField] private ColorChangeComponent _color2;
    
    [SerializeField] private TextMeshProUGUI _modeText;

    private ColorChangeComponent.ColorChangeModeEnum _lastColorMode;

    private void UpdateText()
    {
        var b = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Brightness;
        var c = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Contrast;
        var s = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Saturation;
            
        _modeText.text = $"{(b ? "<color=yellow>" : string.Empty)}B \t Jas<color=white>\n" +
                         $"{(c ? "<color=yellow>" : string.Empty)}C \t Kontrast<color=white>\n" +
                         $"{(s ? "<color=yellow>" : string.Empty)}S \t Saturace";
    }

    private void OnEnable()
    {
        UpdateText();
    }

    private void Update()
    {
        ColorChangeComponent.ColorChangeModeEnum currentMode = _color.enabled ? _color.ColorChangeMode : _color2.ColorChangeMode;
        if (_lastColorMode == currentMode) return;
        
        _lastColorMode = currentMode;
        UpdateText();
    }
}
