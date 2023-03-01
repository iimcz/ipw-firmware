Shader "Unlit/AlignShader"
{
    Properties
    {
        _HorLines("Horizontal lines", Int) = 2
        _VertLines("Vertical lines", Int) = 2

        _LineSize("Line size", Range(0,1)) = 0.05
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

            float _HorLines;
            float _VertLines;
            float enableCurve;

            float _LineSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                _HorLines += 1;
                _VertLines += 1;

                float xPos = fmod(i.uv.x, (1.0 / _HorLines));
                float xWidth = (1.0 / _HorLines) * _LineSize;

                float yPos = fmod(i.uv.y, (1.0 / _VertLines));
                float yWidth = (1.0 / _VertLines) * _LineSize;

                fixed4 col = xPos < xWidth || xPos > ((1.0 / _HorLines) - xWidth) ? fixed4(0, 1, 0, 1) : fixed4(0, 0, 0, 0);
                col += yPos < yWidth || yPos > ((1.0 / _VertLines) - yWidth) ? fixed4(0, 1, 1, 1) : fixed4(0, 0, 0, 0);

                return col;
            }
            ENDCG
        }
    }
}
