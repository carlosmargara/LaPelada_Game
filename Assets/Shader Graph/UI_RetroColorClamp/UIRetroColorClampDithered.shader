Shader "UI/RetroColorClampDithered"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorSteps ("Color Steps", Range(2, 32)) = 4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ColorSteps;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = o.vertex.xy;
                return o;
            }

            // Matriz Bayer 4x4
            float dither4x4(float2 pos)
            {
                int2 ipos = int2(fmod(pos.x, 4), fmod(pos.y, 4));
                int index = ipos.y * 4 + ipos.x;
                float threshold = 0.0;

                float bayer[16] = {
                    0,  8,  2, 10,
                   12,  4, 14,  6,
                    3, 11,  1,  9,
                   15,  7, 13,  5
                };

                threshold = bayer[index] / 16.0;
                return threshold;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // Dithering por canal
                float dither = dither4x4(i.screenPos);

                col.r = floor(col.r * _ColorSteps + dither) / _ColorSteps;
                col.g = floor(col.g * _ColorSteps + dither) / _ColorSteps;
                col.b = floor(col.b * _ColorSteps + dither) / _ColorSteps;

                return col;
            }
            ENDCG
        }
    }
}

