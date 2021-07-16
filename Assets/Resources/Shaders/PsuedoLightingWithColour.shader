Shader "Custom/PsuedoLightingWithColour"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MonoCol("Mono Colour", Color) = (0, 0, 0, 0)
        _MainCol("Main Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 worldPos : TEXCOORD2;
            };

            float3 _PositionData[1000];
            int _PosCount;
            float _BaseLight;

            sampler2D _MainTex;
            fixed4 _MonoCol;
            fixed4 _MainCol;
            int _DualMono;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                float n = step(col.r + col.g + col.b, .1);

                col.rgb = ((_MonoCol * n) + (col.rgb * (1.0 - n))).rgb;
                col.rgb *= ((col.rgb * n) + (_MainCol * (1.0 - n))).rgb;

                col.rgb = ((1 - _DualMono) * col.rgb) +
                    (_DualMono * ((_MonoCol * n) + (_MainCol * (1.0 - n))).rgb);

                col *= i.color;

                float light = 0;
                float3 p;

                for (int j = 0; j < _PosCount; j++) {
                    p = _PositionData[j];
                    float dist = distance(p.xy, i.worldPos.xy);

                    light += pow(p.z, 2) / pow(dist, 2);

                    if (light > 1)
                        break;
                }

                return col * clamp(_BaseLight + light, 0, 1);
            }
            ENDCG
        }
    }
}
