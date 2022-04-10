Shader "Unlit/ProjectorShader"
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

            float enableGeometricCorrection;
            float enableGammaCorrection;
            float enableBrightnessCorrection;
            float enableBlending;

            float flipCurve;
            float crossOver;

            float vertical;

            static fixed3 W = fixed3(0.2125, 0.7154, 0.0721);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                return o;
            }

            float map(float val, float min1, float max1, float min2, float max2)
            {
                return min2 + (val - min1) * (max2 - min2) / (max1 - min1);
            }

            float brightnessCurve(fixed2 uv)
            {
                if (flipCurve < 0.1)
                {
                    if (uv.x > 0.5)
                    {
                        float progress = (uv.x - 0.5) * 2;
                        return -lerp(0, 0.05, progress);
                    }
                }
                else
                {
                    if (uv.x < 0.5)
                    {
                        float progress = uv.x * 2;
                        return -lerp(0.05, 0, progress);
                    }
                }

                return 0.0;
            }

            float easeInOutCubic(float x)
            {
                return x < 0.5 ? 4.0 * x * x * x : 1.0 - pow(-2.0 * x + 2.0, 3.0) / 2.0;
            }

            float blendFunction(float x)
            {
                const float p = 2.0; // constant, but specified separately
                const float under_half = 0.5 * pow(2 * x, p);
                const float over_half = 1 - 0.5 * pow(2 * (1 - x), p);
                if (x < 0.5)
                    return under_half;
                return over_half;
            }

            float gammaCorrection(float x)
            {
                const float inv_gamma = 1.0 / _Gamma;
                return pow(x, inv_gamma);
            }

            float antiOverlap(fixed2 uv)
            {
                float x = clamp(uv.x, 0, 1);
                if (flipCurve < 0.5)
                {
                    if (x > 1.0 - _Cutoff1)
                    {
                        //float progress = lerp(0.0, _Cutoff2, (1.0 - x) / _Cutoff1);
                        //return easeInOutCubic(progress);
                        float progress = map(x, 1.0 - _Cutoff1, 1.0, 1.0, 0.0);
                        return blendFunction(clamp(progress, 0, 1));
                    }
                }
                else
                {
                    if (x < _Cutoff1)
                    {
                        //float progress = lerp(0.0, _Cutoff2, x / _Cutoff1);
                        //return easeInOutCubic(progress);
                        float progress = map(x, 0.0, _Cutoff1, 0.0, 1.0);
                        return blendFunction(clamp(progress, 0, 1));
                    }
                }

                return 1.0;
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
                if (enableBrightnessCorrection > 0.1) bc = 1.0 + brightnessCurve(uv) * 10;

                // Reduce overlap strength in the middle of display to blend the two projections together
                float antiOv = 1.0;
                if (enableBlending > 0.1) antiOv = antiOverlap(uv);

                // Flip image in vertical layout
                if (vertical > 0.1)
                {
                    fixed2 uv_b = uv;

                    uv.x = 1 - uv_b.y;
                    uv.y = uv_b.x;
                }

                fixed4 col = tex.Sample(trilinear_clamp_sampler, uv);

                float intComp = dot(col.rgb, W);
                fixed3 intensity = fixed3(intComp, intComp, intComp);
                col.rgb = lerp(intensity, col.rgb, saturation);

                col.rgb = ((col.rgb - 0.5f) * max(contrast, 0)) + 0.5f;
                col.rgb *= bc;
                col.rgb *= brightness;
                col.rgb *= antiOv;

                return col;
            }
            ENDCG
        }
    }
}
