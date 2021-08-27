// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

// Upgrade NOTE: replaced 'PositionFog()' with transforming position into clip space.
// Upgrade NOTE: replaced 'V2F_POS_FOG' with 'float4 pos : SV_POSITION'

Shader "Custom/Portal Swirl Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainCol ("Main Colour", Color) = (1, 1, 1, 1)
        _SecondaryColor ("Secondary Color", Color) = (0, 0, 0, 0)
        _Random("Random Noise", 2D) = "white" {}
        _Strength("Twirl Strength", float) = 5.
        _Intensity("Worley Intensity", Range(0.001, 10)) = 1.0
        _Size("Size", Range(0.000, 10.0)) = 1.0
        _Offset("Time", float) = 0.0
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always
            Tags { "Queue" = "Transparent" }

            Pass
            {
            Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                uniform float4 _SecondaryColor;
                uniform float _Intensity;
                uniform sampler2D _Random;
                uniform float _Size;
                uniform float _Offset;

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

            float4 getCell3D(const in int x, const in int y, const in int z) {
                float u = (x + y * 31) / 20.0;
                float v = (z - x * 3) / 30.0;
                return tex2D(_Random, float2(u, v));
            }

            float2 cellNoise3D(float3 xyz) {
                int xi = int(floor(xyz.x));
                int yi = int(floor(xyz.y));
                int zi = int(floor(xyz.z));

                float xf = xyz.x - float(xi);
                float yf = xyz.y - float(yi);
                float zf = xyz.z - float(zi);

                float dist1 = 9999999.0;
                float dist2 = 9999999.0;
                float3 cell;

                for (int z = -1; z <= 1; z++) {
                    for (int y = -1; y <= 1; y++) {
                        for (int x = -1; x <= 1; x++) {
                            cell = getCell3D(xi + x, yi + y, zi + z).xyz;
                            cell.x += (float(x) - xf);
                            cell.y += (float(y) - yf);
                            cell.z += (float(z) - zf);
                            float dist = dot(cell, cell);
                            if (dist < dist1) {
                                dist2 = dist1;
                                dist1 = dist;
                            }
                            else if (dist < dist2) {
                                dist2 = dist;
                            }
                        }
                    }
                }

                return float2(sqrt(dist1), sqrt(dist2));
            }

            sampler2D _MainTex;
            float4 _MainCol;
            float _Strength;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 delta = uv - float2(0.5, 0.5);
                float angle = _Strength * length(delta);
                float x = cos(angle) * delta.x - sin(angle) * delta.y;
                float y = sin(angle) * delta.x + cos(angle) * delta.y;
                uv = float2(x + .5, y + .5);

                float2 dists = cellNoise3D((float3(uv.x, uv.y, i.vertex.z) + _Offset * _Time.x) * _Size);
                float4 c = ((_MainCol * dists.x) * (_SecondaryColor * dists.y)) * _Intensity;

                return c * tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
