Shader "Custom/ScreenspaceSquares"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Detail("Base (RGB)", 2D) = "white" {}
		_Scale("Scale", Float) = 10
		_Offset("Offset", Float) = 0
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

			uniform sampler2D _MainTex;
			uniform sampler2D _Detail;
			uniform float _Scale;
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
				float3 color = tex2D(_MainTex, i.uv).rgb;
				float offset = (_Time.x);
				float2 screenUV = i.screenPos.xy / i.screenPos.w + offset;
				screenUV.y += _Offset;
				screenUV *= float2(_Scale * _ScreenParams.x / _ScreenParams.y, _Scale);
				color *= tex2D(_Detail, screenUV).rgb;
				return float4(color, 1);
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
