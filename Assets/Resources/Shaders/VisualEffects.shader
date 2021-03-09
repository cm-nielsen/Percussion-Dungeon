Shader "Custom/VisualEffects"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
		
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

		Tags{ "RenderType" = "Transparent" }

        Pass
        {
				Tags { "LightMode" = "ForwardBase" }
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
			float _ColSpace;
			float _Vignette;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
			
				// vignette
			    col.rgb *= (1 - _Vignette) + ((1.5 - 2 * distance(i.uv.y, 0.5)) * _Vignette);
				
				// colour space reduction
				col.r = floor(col.r * _ColSpace) / _ColSpace;
				col.g = floor(col.g * _ColSpace) / _ColSpace;
				col.b = floor(col.b * _ColSpace) / _ColSpace;
                return col;
            }
            ENDCG
        }
    }
}
