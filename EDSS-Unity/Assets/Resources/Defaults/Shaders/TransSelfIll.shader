//http://forum.unity3d.com/threads/materials-with-both-transparency-and-self-illumination.1554/
Shader "EDSS/TransparentSelfIllumination" 
{
	Properties {
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Color",Color) = (0.5,0.5,0.5,1)
	}


Subshader {
	Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
	//Material {  Emission [_Color]  }

 Pass {
	  //ZTest Always Cull Off ZWrite On
	  //ZTest Always
	  //ZWrite Off
	  Fog { Mode off }      
	  //ColorMask RGBA
	  //Cull Front
	  Blend SrcAlpha OneMinusSrcAlpha
	  BindChannels
	  {
		Bind "Color", color
		Bind "Vertex", vertex
		Bind "TexCoord", texcoord
	  }

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
		//half2 uv2 : TEXCOORD1;
		fixed4 color : COLOR;
	};

	struct appdata
	{
		float4 vertex : POSITION;
		half2 uv : TEXCOORD0;
		//half2 uv2 : TEXCOORD1;
		fixed4 color : COLOR;
	};
				
	sampler2D _MainTex;

	fixed4 _MainTex_ST;
	fixed4 _Color;

	v2f vert (appdata v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//o.uv2 = TRANSFORM_TEX(v.uv2, _TintTex);
		//o.color = v.color;
		return o;
	}

	fixed4 frag(v2f i) : COLOR
	{
		//fixed4 c = tex2D(_MainTex, i.uv) * _Color;
		fixed4 c = _Color;
		//fixed4 t = tex2D(_TintTex, i.uv2);
		//clip(c.a - 0.5f);
		//c.a *= 0;
		c.a = _Color.a;//
		//c.rgb = BlendOverlay(c.rgb, i.color.rgb);
		//c.rgb = c.rgb * t.rgb;
		return c;
	}
		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	 		 	 	  	 	  	 	  	 	
      ENDCG
  	}

  }

	Fallback "Transparent/VertexLit"
}
