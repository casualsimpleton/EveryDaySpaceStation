//http://answers.unity3d.com/questions/292381/invert-filter-reticule.html
Shader "EDSS/InvertColor" 
{ 
	Properties 
	{ 
		_MainTex ("Alpha (A) only", 2D) = "white" {} 
		_AlphaCutOff ("Alpha cut off", Range(0,1)) = 1 
	} 
	
	SubShader 
	{ 
		Tags { "Queue" = "Transparent+10" } 
		Pass 
		{ 
			Fog { Mode Off } 
			Blend OneMinusDstColor Zero 
			ZWrite Off

             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
     
             sampler2D _MainTex;
             float _AlphaCutOff;
             struct appdata
             {
                 float4 vertex : POSITION;
                 float4 texcoord : TEXCOORD0;
             };
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float4 uv : TEXCOORD0;
             };
             v2f vert (appdata v)
             {
                 v2f o;
                 o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                 o.uv = float4(v.texcoord.xy, 0, 0);
                 return o;
             }
             half4 frag( v2f i ) : COLOR
             {
                 half4 c = 1;
                 c.a = tex2D(_MainTex, i.uv.xy).a;
                 clip(_AlphaCutOff - c.a);
                 return c;
             }
             ENDCG
         }
     }
}