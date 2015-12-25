Shader "Custom/ScreenspaceSquares"
{
	Properties
	{
		_BackgroundTex("Background (RGB)", 2D) = "white" {}
		_BackgroundColor("Background Color", Color) = (1,1,1,1)
		_Color1("Color1", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (0,0,0,1)
		_Scale("Scale (Float)", Float) = 16
		_TexScale("Texture Scale (Float)", Float) = 1
		_Offset("Offset (Float)", Float) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		Cull Off

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#pragma target 3.0

			struct v2f
			{
				float4 pos       : POSITION;
				float2 uv        : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
			};

			uniform sampler2D _BackgroundTex;
			uniform float4 _BackgroundColor;
			uniform float4 _Color1;
			uniform float4 _Color2;
			uniform float _Scale;
			uniform float _TexScale;
			uniform float _Offset;

			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
				o.screenPos = ComputeScreenPos(o.pos);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float2 screenUV = i.screenPos.xy / i.screenPos.w;
				float aspectRatio = _ScreenParams.x / _ScreenParams.y;

				// Screen UV is offset by the camera position
				screenUV.y += _Offset;

				// Multiply background texture to the main texture
				float4 color = tex2D(_BackgroundTex, (screenUV * float2(aspectRatio, 1)) * _TexScale) * _BackgroundColor;

				// Add more offset; this is used by the squares
				screenUV += _Time.x;
				screenUV *= float2(_Scale * aspectRatio, _Scale);

				// Draw the squares
				float scale = _ScreenParams.y;
				float2 uv = floor(scale * screenUV * float2(aspectRatio, 1) / _ScreenParams.xy);
				float colorValue = fmod(uv.x + uv.y, 2);
				float4 squareColor = float4(lerp(_Color1, _Color2, colorValue));

				color *= squareColor;

				return color;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
