Shader "Unlit/RampShader"
{
    Properties
    {
        _Vertical("Vertical", Range(0, 1)) = 0.0
    }

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Vertical;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float val = i.uv.x;
                if (_Vertical)
                {
                    val = i.uv.y;
                }
                float lightness = (val % (1.0 / 4.0)) * 4.0;
                
                if (val < 0.25)
                {
                    return fixed4(lightness, lightness, lightness, 1);
                }
                else if (val < 0.50)
                {
                    return fixed4(1, lightness, lightness, 1);
                }
                else if (val < 0.75)
                {
                    return fixed4(lightness, 1, lightness, 1);
                }
                else
                {
                    return fixed4(lightness, lightness, 1, 1);
                }
            }
            ENDCG
        }
    }
}
