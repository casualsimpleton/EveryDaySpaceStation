Shader "EDSS/ZWrite-Transparent"
{
	Properties
	{
		_BaseTexture ("Base Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 300

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite On
		Cull Back

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _BaseTexture;

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			float4 _BaseTexture_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _BaseTexture);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				half4 texcol = tex2D (_BaseTexture, i.texcoord);
				return texcol;
			}
			ENDCG
		}

	}

	Fallback "Transparent/VertexLit"
}