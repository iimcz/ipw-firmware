Shader "Unlit/GridShader"
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float xPos = trunc(i.uv.x * 30);
                float yPos = trunc(i.uv.y * 30);

                if (yPos % 2 == 0) xPos++;

                return lerp(fixed4(1, 1, 1, 1), fixed4(0, 0, 0, 0), xPos % 2);
            }
            ENDCG
        }
    }
}
