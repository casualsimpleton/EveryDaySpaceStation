Shader "EDSS/ZWrite-Transparent"
{
	Properties {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Color",Color) = (1,1,1,1)
	}

Subshader {
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

 Pass {
	  //ZTest Always Cull Off ZWrite On
	  Fog { Mode off }      
	  ColorMask RGBA
	  Cull Back

      CGPROGRAM

	#pragma vertex vert
	#pragma fragment frag
	//#pragma only_renderers d3d9

	#include "UnityCG.cginc"
	struct v2f 
	{
		float4 pos : SV_POSITION;
		half2 uv  : TEXCOORD0;
	};
				
	sampler2D _MainTex;

	fixed4 _MainTex_ST;
	fixed4 _Color;

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		fixed4 c = tex2D(_MainTex,i.uv) * _Color;
		clip(c.a - 0.5f);
		c.a *= 0;
		return c;
	}
		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	
      ENDCG
  	}

  }

	Fallback "Transparent/VertexLit"
}