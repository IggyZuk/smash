Shader "Custom/CRTShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
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
				uniform float _PixelSize;

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
					half4 color = tex2D(_MainTex, i.uv);

					float2 ps = i.scr_pos.xy * _ScreenParams.xy / i.scr_pos.w;

					float4 outcolor = color;

					int px = ((int)ps.x % _PixelSize);
					if (px == 1) outcolor = color * 1.25;

					int py = ((int)ps.y % _PixelSize) + _Time;
					if (py == _PixelSize - 1) outcolor = color * 1.25;

					if (px == 0 || py == 0) outcolor = color - 0.2;

					return outcolor;
				}

				ENDCG
			}
		}
			FallBack "Diffuse"
}
