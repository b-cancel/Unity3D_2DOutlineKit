
//Bryan Cancel's Outline

Shader "Custom/Ultimate_2D_Outline" {

	Properties
	{
		[HideInInspector] _MainTex("Sprite Texture", 2D) = "white" {}

		//--- Per Shader Variables

		//--- In Sprite Outline
		[PerRendererData] _Outline("Outline Toggle", int) = 1
		[PerRendererData] _OutlineColor("Outline Color", Color) = (0, 0, 0, 0)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1

		//--- Around Sprite Outline
		

		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

	SubShader
	{
		Tags
		{
			//RENDERING ORDER --- NOTE: Overlay (above all), Transparent (in the layer of that object)
			"Queue" = "Overlay" //"Overlay" (4,000), "Transparent" (3,000), "AlphaTest" (2,450), "Geometry" (2,000), "Background" (1,000)

			//??? -> IDK what this does
			//"RenderType" = "Geometry" //https://docs.unity3d.com/Manual/SL-ShaderReplacement.html

			//DisableBatching -> not needed

			//ForceNoShadowCasting -> not needed

			//??? -> IDK if this being off when sprites are NOT semi Transparent does anything...
			"IgnoreProjector" = "True" //https://docs.unity3d.com/Manual/class-Projector.html --- Since Our Sprites may be Semi Transparent we have this on
			
			"CanUseSpriteAtlas" = "True" //TODO... test if this works as desired

			"PreviewType" = "Plane" //When something in broken we view a plane (helpful for debugging)
		}

		Name "DrawOutlineInSprite"
		//LOD 100 //standard level of detail // -> not needed right now
		Cull Off //Back | Front | Off
		//ZTest(Less | Greater | LEqual | GEqual | Equal | NotEqual | Always) // -> How should depth testing be performed. Default is LEqual (draw objects in from or at the distance as existing objects; hide objects behind them).
		ZWrite Off //On | Off // -> Controls whether pixels from this object are written to the depth buffer (default is On). If you’re drawng solid objects, leave this on. If you’re drawing semitransparent effects, switch to ZWrite Off. For more details read below.
		//Offset OffsetFactor, OffsetUnits
		Blend One OneMinusSrcAlpha //Other Options are very complicated and often require DX11
		//AlphaToMask On | Off
		//ColorMask RGB | A | 0 | any combination of R, G, B, A
		Lighting Off //On | Off //NOTE: -> For the settings defined in the Material block to have any effect, you must enable Lighting with the Lighting On command. If lighting is off instead, the color is taken straight from the Color command.
		//Material{ Material Block } // -> The Material block is used to define the material properties of the object.
		//SeparateSpecular On | Off
		//Color Color - value // -> Sets the object to a solid color
		//ColorMaterial AmbientAndDiffuse | Emission // -> Makes per-vertex color be used instead of the colors set in the material
		Fog{ Mode Off } // - > fogging blends the color of the generated pixels down towards a constant color based on distance from camera
		//AlphaTest(Less | Greater | LEqual | GEqual | Equal | NotEqual | Always) CutoffValue

		//SetTexture textureProperty { combine options }

		Pass //"DrawOutlineInSprite" //this allows us to reuse this pass with the usepass command if needed
		{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				float _Outline;
				fixed4 _OutlineColor;
				int _OutlineSize;
				float4 _MainTex_TexelSize;

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

					// If outline is enabled and there is a pixel, try to draw an outline.
					if (_Outline > 0 && c.a != 0) {
						float totalAlpha = 1.0;

						[loop]
						for (int i = 1; i < _OutlineSize + 1; i++) {
							fixed4 pixelUp = tex2D(_MainTex, IN.texcoord +  fixed2(0, i * _MainTex_TexelSize.y));
							fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i *  _MainTex_TexelSize.y));
							fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2( i * _MainTex_TexelSize.x, 0));
							fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2( i * _MainTex_TexelSize.x, 0));

							totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
						}

						if (totalAlpha == 0) {
							c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
						}
					}

					c.rgb *= c.a;

					return c;
				}
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}