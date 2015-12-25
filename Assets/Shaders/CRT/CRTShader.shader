Shader "Custom/CRTShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_PixelColor("Pixel Color", Color) = (1,1,1,1)
		_PixelSize("Pixel Size", Float) = 3
	}

		SubShader{
			Pass {
				ZTest Always Cull Off ZWrite Off Fog { Mode off }

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#pragma target 3.0

				struct v2f
				{
					float4 pos      : POSITION;
					float2 uv       : TEXCOORD0;
					float4 scr_pos : TEXCOORD1;
				};

				uniform sampler2D _MainTex;
				uniform float4 _PixelColor;
				uniform float _PixelSize;

				float hash(float n)
				{
					return frac(sin(n)*43758.5453);
				}

				float noise(float3 x)
				{
					// The noise function returns a value in the range -1.0f -> 1.0f

					float3 p = floor(x);
					float3 f = frac(x);

					f = f*f*(3.0 - 2.0*f);
					float n = p.x + p.y*57.0 + 113.0*p.z;

					return lerp(lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
						lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y),
						lerp(lerp(hash(n + 113.0), hash(n + 114.0), f.x),
						lerp(hash(n + 170.0), hash(n + 171.0), f.x), f.y), f.z);
				}

				v2f vert(appdata_img v)
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
					o.scr_pos = ComputeScreenPos(o.pos);
					return o;
				}

				half4 frag(v2f i) : COLOR
				{ 
					float2 uv = i.uv;

					float2 ps = i.scr_pos.xy * _ScreenParams.xy / i.scr_pos.w;

					uv.x = (floor(ps.x / _PixelSize) / _ScreenParams.x) * _PixelSize;
					uv.y = 1 - (floor(ps.y / _PixelSize) / _ScreenParams.y) * _PixelSize;

					//uv.y = floor(i.scr_pos.y / 2) * _ScreenParams.y;

					//if ((int)(ps.y + _Time.a) % 4 == 0) uv.y -= 0.05 * noise(ps.x * 0.001 + 0.005 + _Time.a);
					//if ((int)(ps.x + sin(_Time.a) * 10) % 2 == 0) uv.x += 0.1 * noise(ps.x * 0.01 + _Time.a);

					half4 color = tex2D(_MainTex, uv);
					float4 outcolor = color;



					int px = ((int)ps.x % _PixelSize);
					if (px == 1) outcolor = color * 1.25;

					int py = ((int)ps.y % _PixelSize) + _Time;
					if (py == _PixelSize - 1) outcolor = color * 1.25;

					if (px == 0 || py == 0) outcolor = color - 0.2;



					/*if ((int)(ps.x) % _PixelSize == 0) outcolor += _PixelColor;
					if ((int)(ps.y) % _PixelSize == 0) outcolor += _PixelColor;

					if ((int)(ps.y) % (_PixelSize * 4) == 0) outcolor += _PixelColor * 1.5;
					if ((int)(ps.x) % (_PixelSize * 4) == 0) outcolor += _PixelColor * 1.5;*/

					return outcolor;
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}
