Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "defaulttexture"{}

		_Foo ("Foo", Range(0, 1)) = 0.5
		_MonoCol("Mono Colour", Color) = (0, 0, 0, 0)
		_MainCol("Main Colour", Color) = (1, 1, 1, 1)
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
			sampler2D _NoiseTex;
			fixed3 _MainCol;
			fixed3 _MonoCol;
			float _Foo;
			int _DualMono;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
				//dissolve effect
				fixed4 nos = tex2D(_NoiseTex, i.uv);
				if (nos.r + nos.b + nos.g > 3 * _Foo)
					col.a = 0;

				// colour customization
				float n = step(col.r + col.g + col.b, .1);

				col.rgb = ((_MonoCol * n) + (col.rgb * (1.0 - n))).rgb;
				col.rgb *= ((col.rgb * n) + (_MainCol * (1.0 - n))).rgb;

				col.rgb = ((1 - _DualMono) * col.rgb) +
					(_DualMono * ((_MonoCol * n) + (_MainCol * (1.0 - n))).rgb);

                return col;
            }
            ENDCG
        }
    }
}
