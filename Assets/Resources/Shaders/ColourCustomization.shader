Shader "Custom/ColourCustomization"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MonoCol ("Mono Colour", Color) = (0, 0, 0, 0)
		_MainCol("Main Colour", Color) = (1, 1, 1, 1)
        _RandCol("Random Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags{
			"Queue" = "Transparent"
		}
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

            sampler2D _MainTex;
			fixed4 _MonoCol;
			fixed4 _MainCol;
            fixed4 _RandCol;
			int _DualMono;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				//if (col.a > 0 && col.r == 0 && col.g == 0 && col.b == 0)
				//	col = _MonoCol;
				//else
				//	col *= _MainCol;

				float n = step(col.r + col.g + col.b, .1);

				col.rgb = ((_MonoCol * n) + (col.rgb * (1.0 - n))).rgb;
				col.rgb *= ((col.rgb * n) + (_MainCol * (1.0 - n))).rgb;

				col.rgb = ((1 - _DualMono) * col.rgb) +
					(_DualMono * ((_MonoCol * n) + (_MainCol* (1.0 - n))).rgb);

                return col * _RandCol;
            }
            ENDCG
        }
    }
}
