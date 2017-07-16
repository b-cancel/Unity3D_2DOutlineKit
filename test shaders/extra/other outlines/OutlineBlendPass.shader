// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outline/OutlineBlendPass" {
	Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "white" {}
		[HideInInspector] _OutlineBuffer ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass
         {
             Name "OutlineBlendPass"
             Blend Off
             AlphaTest Off
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag            
             #include "UnityCG.cginc"
             
             sampler2D _MainTex;
             sampler2D _OutlineBuffer;
             
             float4 color0;
             float4 color1;
             float4 color2;
             float4 color3;
             
             struct v2f
             {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
             };
             
             float4 _MainTex_ST;
             
             v2f vert (appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos(v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                 return o;
             }
             
             float4 frag (v2f i) : COLOR
             {
             	 float4 background = tex2D(_MainTex, i.uv);
 				 float4 factor = tex2D(_OutlineBuffer, i.uv);
 				 
 				 float4 c;
 				 if(factor.r > 0.1 && factor.r < 0.9) {
 				 	c = color0;
			 	 }else if(factor.g > 0.1 && factor.g < 0.9) {
 				 	c = color1;
			 	 }else if(factor.b > 0.1 && factor.b < 0.9) {
			 	 	c = color2;
			 	 }else if(factor.a > 0.1 && factor.a < 0.9) {
			 	 	c = color3;
 				 }else{
 				 	c = background;
 				 }
 				 return c;
             }
             ENDCG
         }
	}
}
