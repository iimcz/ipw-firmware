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
        var j = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Brightness;
        var r = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.BrightnessRed;
        var g = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.BrightnessGreen;
        var b = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.BrightnessBlue;
        
        var c = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Contrast;
        var s = _lastColorMode == ColorChangeComponent.ColorChangeModeEnum.Saturation;
            
        _modeText.text = $"Stisknutím níže uvedených kláves změníte momentálně upravovanou hodnotu\n" +
                         $"{(j ? "<color=yellow>" : string.Empty)}J \t Jas (Vše)<color=white>\n" +
                         $"{(r ? "<color=yellow>" : string.Empty)}R \t Jas (R)<color=white>\n" +
                         $"{(g ? "<color=yellow>" : string.Empty)}G \t Jas (G)<color=white>\n" +
                         $"{(b ? "<color=yellow>" : string.Empty)}B \t Jas (B)<color=white>\n" +
                         $"{(c ? "<color=yellow>" : string.Empty)}C \t Kontrast<color=white>\n" +
                         $"{(s ? "<color=yellow>" : string.Empty)}S \t Saturace";
    }

    private void OnEnable()
    {
        UpdateText();
        ProjectorTransformationPass.EnableColorRamp = true;
        ProjectorTransformationPass.EnableBlending = false;
        ProjectorTransformationPass.EnableContrastSaturation = false;
    }

    private void OnDisable()
    {
        ProjectorTransformationPass.EnableColorRamp = false;
        ProjectorTransformationPass.EnableBlending = true;
        ProjectorTransformationPass.EnableContrastSaturation = true;
    }

    private void Update()
    {
        ColorChangeComponent.ColorChangeModeEnum currentMode = _color.enabled ? _color.ColorChangeMode : _color2.ColorChangeMode;
        if (_lastColorMode == currentMode) return;
        
        _lastColorMode = currentMode;
        UpdateText();
    }
}
