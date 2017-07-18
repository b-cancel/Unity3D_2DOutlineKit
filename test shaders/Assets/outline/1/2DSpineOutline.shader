// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/2DSpineOutline"
{
	Properties
	{
		_MainTex("Base (RGBA)", 2D) = "white" {}
		_OutlineColor("OutlineColor", Color) = (0, 0, 0, 0)
		Padding("Padding", Range(0, 1)) = 0.5
	}

		SubShader
	{
		Tags { "Queue"="Overlay" "IgnoreProjector"="True"  }
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha //cant change to... One OneMinusSrcAlpha
		Lighting Off

		Pass
		{
			Name "BACK_SPACE"
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			//#pragma enable_d3d11_debug_symbols
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv_min : ANY0;
				float2 uv_max : ANY1;
			};

			uniform float4 _MainTex_TexelSize;

			// vertex shader in MVT matrix Without computation geometry shader proceed from [rough korean translation]
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = v.vertex;
				o.uv = v.uv;
				o.uv_min = float2(0, 0);
				o.uv_max = float2(0, 0);

				return o;
			}

			uniform float Padding;

			// To draw an outline, you need to draw it slightly larger than the assigned texture area
			// For that task, increase the vertex value of quad by a little bit
			[maxvertexcount(3)]
			void geom(triangle v2f v[3], inout TriangleStream<v2f> triStream)
			{
				const float2 center = (v[0].vertex.xy + v[1].vertex.xy + v[2].vertex.xy) / 3;
				const float2 uv_center = (v[0].uv + v[1].uv + v[2].uv) / 3;

				float2 uv_min = float2(1,1);
				float2 uv_max = float2(0,0); 

				for (int i = 0; i < 3; ++i)
				{
					uv_min = min(uv_min, v[i].uv);
					uv_max = max(uv_max, v[i].uv);
				}

				v2f o;

				for (i = 0; i < 3; ++i)
				{
					o.vertex = v[i].vertex;

					float2 scaleNormal = normalize(o.vertex.xy - center);
					o.vertex.xy += scaleNormal * Padding;
					float2 scaled = (o.vertex.xy - center) / (v[i].vertex.xy - center);
					o.vertex = UnityObjectToClipPos(o.vertex);

					o.uv = v[i].uv;
					o.uv -= uv_center;
					o.uv *= scaled;
					o.uv += uv_center;

					o.uv_min = uv_min;
					o.uv_max = uv_max;

					triStream.Append(o);
				}
			}
			
			sampler2D _MainTex;
			uniform fixed4 _OutlineColor;

			float getAlpha(float2 uv, float2 uv_min, float2 uv_max)
			{
				float ret = 0;

				// TODO @sangmoon Use another control statement instead of an if statement
				if (uv_min.x <= uv.x && uv.x <= uv_max.x
					&& uv_min.y <= uv.y && uv.y <= uv_max.y)
				{
					ret = tex2D(_MainTex, uv).a;
				}
				
				return ret;
			}

			// Search with circle and search for alpha value around the pixel
			// Enter the color if alpha is found 
			fixed4 frag (v2f frag) : SV_Target
			{
				float circleAlpha = 0.0;
				float2 stepVector = (_MainTex_TexelSize.xy * 0.5);
				float stepSize = sqrt(dot(stepVector, stepVector));

				const float angle = 3.141592 * 0.125;
				float2 stepPosition = float2(0, stepSize);
				float2x2 rotationMatrix = float2x2(cos(angle), -sin(angle), sin(angle), cos(angle));

				for (int i = 0; i < 16; ++i)
				{
					circleAlpha += getAlpha(frag.uv + stepPosition * 2, frag.uv_min, frag.uv_max);
					circleAlpha += getAlpha(frag.uv + stepPosition * 5, frag.uv_min, frag.uv_max);

					stepPosition = mul(rotationMatrix, stepPosition);
				}

				// TOOD @sangmoon Use the added alpha total + smoothstep to let the outline fall naturally			
				circleAlpha = clamp(circleAlpha, 0.0, 1.0);
				
				return float4(_OutlineColor.rgb, circleAlpha);
			}

			ENDCG
		}

		Pass
		{
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]{
				Combine texture * primary
			}
		}
	}
}