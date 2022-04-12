Shader "Unlit/CalibrationShader"
{
    Properties
    {
        _Power("Power", Range(1,3)) = 2
        _Power2("Power2", Range(1,3)) = 2
        
        _Cutoff1("Cutoff", Range(0,1)) = 1
        _Cutoff2("Cutoff 2", Range(0,1)) = 1

        _DistMinTop("Minimal Distance (top projector, meters)", Range(0, 4)) = 1.7
        _DistMaxTop("Maximum Distance (top projector, meters)", Range(0, 4)) = 2.2
        
        _DistMinBot("Minimal Distance (bottom projector, meters)", Range(0, 4)) = 1.7
        _DistMaxBot("Maximum Distance (bottom projector, meters)", Range(0, 4)) = 2.2
        
        _Gamma("Gamma", Range(0, 3)) = 2.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"
            #include "IPWFunctions.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _Power;
            float _Power2;
            
            float _Cutoff1;
            float _Cutoff2;

            float _DistMinTop;
            float _DistMaxTop;
        
            float _DistMinBot;
            float _DistMaxBot;

            float _Gamma;

            SamplerState trilinear_clamp_sampler;

            Texture2D tex;
            float contrast;
            float4 brightness;
            float saturation;

            float enableColorRamp;
            float enableGeometricCorrection;
            float enableGammaCorrection;
            float enableBrightnessCorrection;
            float enableContrastSaturation;
            float enableBlending;

            float flipCurve;
            float crossOver;

            float vertical;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Perspective correct UV mapping
                fixed2 uv = i.uv / i.uv2;
                
                // Stretch sides of texture due to projector angle
                if (enableGeometricCorrection) {
                    if (flipCurve < 0.5) {
                        uv.x = 3.0 * uv.x - pow(uv.x, _Power2);
                        uv.x /= 2.0;
                    }
                    else {
                        uv.x = uv.x + pow(uv.x, _Power);
                        uv.x /= 2.0;
                    }
                }

                // Compensate for uneven projector brightness on edges
                float bc = 0;
                if (enableBrightnessCorrection > 0.1) bc = 1.0 + brightnessCurve(uv, flipCurve) * 10;

                // Reduce overlap strength in the middle of display to blend the two projections together
                float antiOv = 1.0;
                if (enableBlending > 0.1) antiOv = antiOverlap(uv, _Cutoff1, flipCurve);

                if (enableGammaCorrection > 0.1)
                {
                    bc = gammaCorrection(bc, _Gamma);
                    antiOv = gammaCorrection(antiOv, _Gamma);
                }

                // Flip image in vertical layout
                float rampValue = uv.y;
                if (vertical > 0.1)
                {
                    fixed2 uv_b = uv;

                    uv.x = 1 - uv_b.y;
                    uv.y = uv_b.x;

                    rampValue = uv.x;
                }

                fixed4 col = tex.Sample(trilinear_clamp_sampler, uv);
                if (enableColorRamp > 0.1)
                {
                    col = colorRamp(rampValue);
                }

                if (enableContrastSaturation > 0.1)
                {
                    col.rgb = applySaturation(col.rgb, saturation);
                    col.rgb = applyContrast(col.rgb, contrast);
                }
                col.rgb *= bc;
                col.rgb *= brightness;
                col.rgb *= antiOv;

                return col;
            }
            ENDCG
        }
    }
}
