Shader "Custom/PsuedoLighting"
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
        Blend SrcAlpha OneMinusSrcAlpha
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

            float _PositionData[10000];
            int _PosArrayWidth;
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
                
                //Apply Colour Customization effect
                float n = step(col.r + col.g + col.b, .1);

                col.rgb = ((_MonoCol * n) + (col.rgb * (1.0 - n))).rgb;
                col.rgb *= ((col.rgb * n) + (_MainCol * (1.0 - n))).rgb;

                col.rgb = ((1 - _DualMono) * col.rgb) +
                    (_DualMono * ((_MonoCol * n) + (_MainCol * (1.0 - n))).rgb);

                col *= i.color;

                //Apply PsuedoLighting
                float2 wPos = i.worldPos.xy;
                float light = _PositionData[wPos.x * _PosArrayWidth + wPos.y];
                float3 p;

                //for (int j = 0; j < _PosCount; j++) {
                //    p = _PositionData[j];
                //    //float dist = distance(p.xy, i.worldPos.xy);
                //    float dist = pow(p.x - wPos.x, 2) + pow(p.y - wPos.y, 2);

                //    light += p.z / dist;
                //}

                //col.xyz *=  clamp(_BaseLight + light, 0, 1);
                return col;
            }
            ENDCG
        }
    }
}
