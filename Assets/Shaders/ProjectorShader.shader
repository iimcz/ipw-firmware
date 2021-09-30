Shader "Unlit/NewUnlitShader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            
            SamplerState trilinear_clamp_sampler;

            Texture2D tex;
            float contrast;
            float brightness;
            float saturation;

            float enableCurve;
            float flipCurve;

            static fixed3 W = fixed3(0.2125, 0.7154, 0.0721);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv2;
                return o;
            }

            float brightnessCurve(fixed2 uv)
            {
                if (flipCurve > 0.1)
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

            fixed4 frag(v2f i) : SV_Target
            {
                fixed2 uv = i.uv / i.uv2;
                fixed4 col = tex.Sample(trilinear_clamp_sampler, uv);

                float bc = 0;
                // TODO: Vertical orientation
                // TODO: Variable control points
                if (enableCurve > 0.1) bc = brightnessCurve(uv);

                float intComp = dot(col.rgb, W);
                fixed3 intensity = fixed3(intComp, intComp, intComp);
                col.rgb = lerp(intensity, col.rgb, saturation);

                col.rgb = ((col.rgb - 0.5f) * max(contrast, 0)) + 0.5f;
                col.rgb += brightness + bc;

                return col;
            }
            ENDCG
        }
    }
}
