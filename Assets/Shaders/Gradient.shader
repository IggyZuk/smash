Shader "Custom/Gradient" {

	Properties {
		_ColorTop("Top Color", Color) = (1,1,1,1)
		_ColorBottom("Bottom Color", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 1
		_Offset("Offset", Float) = 0.5
	}

	SubShader {
		Tags {"Queue" = "Background"  "IgnoreProjector" = "True"}
		LOD 100

		ZWrite On

		Pass {
			CGPROGRAM
			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _ColorTop;
			fixed4 _ColorBottom;
			fixed  _Scale;
			fixed  _Offset;

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 col : COLOR;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.col = lerp(_ColorBottom, _ColorTop, v.vertex.y * _Scale + _Offset);
				return o;
			}

			float4 frag(v2f i) : COLOR {
				float4 c = i.col;
				return c;
			}
			ENDCG
		}
	}

}
