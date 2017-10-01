// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/SpriteOutline"
{
	 Properties
    {
     	[PerRendererData] _ObjectSize ("Local Size",Vector) = (1,1,1,1) // x,y : 1/local scale of object
        [PerRendererData] _MainTex  ("MainTex", 2D) = ""{}
        [PerRendererData] _Distance ("Distance", Float) = 1
    }

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Geometry" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
        Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			sampler2D _MainTex;
    		fixed4 _MainTex_TexelSize;
    		fixed _Distance;
    		fixed4 _ObjectSize;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				return OUT;
			}

			fixed4 frag(v2f i) : SV_Target{

			float width =  _Distance*_ObjectSize.x*_MainTex_TexelSize.x;
			float height =  _Distance*_ObjectSize.y*_MainTex_TexelSize.y;

	        half a1 = tex2D(_MainTex, i.texcoord + float2(-width, -height)).a;
	        half a2 = tex2D(_MainTex, i.texcoord + float2( 0, -height)).a;
	        half a3 = tex2D(_MainTex, i.texcoord + float2(+width, -height)).a;

	        half a4 = tex2D(_MainTex, i.texcoord + float2(-width,  0)).a;
	        half a6 = tex2D(_MainTex, i.texcoord + float2(+width,  0)).a;

	        half a7 = tex2D(_MainTex, i.texcoord + float2(-width, +height)).a;
	        half a8 = tex2D(_MainTex, i.texcoord + float2( 0, +height)).a;
	        half a9 = tex2D(_MainTex, i.texcoord + float2(+width, +height)).a;

	        half gx = - a1 - a2*2 - a3 + a7 + a8*2 + a9;
	        half gy = - a1 - a4*2 - a7 + a3 + a6*2 + a9;

	        half w = sqrt(gx * gx + gy * gy)*1.25;

	        w = min(w,1);
	       
	        i.color *= i.color.a*w;
		    return (i.color);

			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
