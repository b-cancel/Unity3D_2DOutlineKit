// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outline/OutlineBlurPass" {
	Properties {
		[HideInInspector] _MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
	
         Pass
         {
             Name "HorizontalBlur"
             Blend Off
             AlphaTest Off
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag            
             #include "UnityCG.cginc"
             
             sampler2D _MainTex;
             
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
                float blurAmount = 0.00035;
 
 				float4 fragment = tex2D(_MainTex, i.uv);
                float4 sum = half4(0.0, 0.0, 0.0, 0.0);
                
 				sum += tex2D(_MainTex, float2(i.uv.x - 4.0 * blurAmount, i.uv.y)) * 0.05;
                sum += tex2D(_MainTex, float2(i.uv.x - 3.0 * blurAmount, i.uv.y)) * 0.09;
                sum += tex2D(_MainTex, float2(i.uv.x - 2.0 * blurAmount, i.uv.y)) * 0.12;
                sum += tex2D(_MainTex, float2(i.uv.x - blurAmount, i.uv.y)) * 0.15;
                sum += fragment * 0.16;
                sum += tex2D(_MainTex, float2(i.uv.x + blurAmount, i.uv.y)) * 0.15;
                sum += tex2D(_MainTex, float2(i.uv.x + 2.0 * blurAmount, i.uv.y)) * 0.12;
                sum += tex2D(_MainTex, float2(i.uv.x + 3.0 * blurAmount, i.uv.y)) * 0.09;
                sum += tex2D(_MainTex, float2(i.uv.x + 4.0 * blurAmount, i.uv.y)) * 0.05;
 
                return sum;
             }
             ENDCG
         }
         
         Pass
         {
             Name "VerticalBlur"
 
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag            
             #include "UnityCG.cginc"
             
             sampler2D _MainTex;
             
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
                 float blurAmount = 0.00035;
 
 				 float4 fragment = tex2D(_MainTex, i.uv);
                 float4 sum = half4(0.0, 0.0, 0.0, 0.0);
 
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 4.0 * blurAmount)) * 0.05;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 3.0 * blurAmount)) * 0.09;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - 2.0 * blurAmount)) * 0.12;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y - blurAmount)) * 0.15;
                 sum += fragment * 0.16;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + blurAmount)) * 0.15;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 2.0 * blurAmount)) * 0.12;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 3.0 * blurAmount)) * 0.09;
                 sum += tex2D(_MainTex, float2(i.uv.x, i.uv.y + 4.0 * blurAmount)) * 0.05;

                 return sum;
             }
             ENDCG
         }
	}
}
