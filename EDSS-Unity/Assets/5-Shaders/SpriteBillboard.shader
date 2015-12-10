//https://en.wikibooks.org/wiki/Cg_Programming/Unity/Billboards
Shader "EDSS/SpriteBillboard" {
   Properties {
      _MainTex ("Texture Image", 2D) = "white" {}
	  _Color ("Color", Color) = (1,1,1,1)
	  _Scale ("Scale", Vector) = (1.0, 1.0, 1.0)
   }
   SubShader {
      Pass {   
         CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
	//#pragma only_renderers d3d9
	#define BlendOverlay(base, blend) 	(base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)))

	#include "UnityCG.cginc"
	struct v2f 
	{
		float4 pos : SV_POSITION;
		half2 uv  : TEXCOORD0;
		fixed4 color : COLOR;
	};

	struct appdata
	{
		float4 vertex : POSITION;
		half2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};
				
	sampler2D _MainTex;

	fixed4 _MainTex_ST;
	fixed4 _Color;
	float3 _Scale = float3(1.0, 1.0, 1.0);

	v2f vert (appdata v)
	{
		v2f o;
		//o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.pos = mul(UNITY_MATRIX_P, mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
			- float4(v.vertex.x * -_Scale.x * 0.5, v.vertex.y * -_Scale.y * 0.5, 0.0, 0.0));
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex,i.uv) * _Color;
		clip(c.a - 0.5f);
		c.a *= 0;
		c.rgb = BlendOverlay(c.rgb, i.color.rgb);
		return c;
	}
		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	
      ENDCG
      }
   }
}
