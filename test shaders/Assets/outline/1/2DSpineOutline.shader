// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/2DSpineOutline"
{
	Properties
	{
		_MainTex("Base (RGBA)", 2D) = "white" {}
		_OutlineColor("OutlineColor", Color) = (0, 0, 0, 0)
		StepStrength("StepStrength", Range(1, 5)) = 1
		Padding("Padding", Range(0, 1)) = 0.5
	}

		SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True"  }
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
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

			// vertex shader에서 MVT matrix 연산을 하지 않고 geometry shader 에서 진행 
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

			// 아웃라인을 그리려면 할당받은 texture영역보다 살짝 크게 그려줘야 한다
			// 해당 작업을 위하여, quad의 vertex값을 살짝 늘려 그릴 수 있는 영억을 늘린다 
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
			uniform float StepStrength;

			float getAlpha(float2 uv, float2 uv_min, float2 uv_max)
			{
				float ret = 0;

				// TODO @sangmoon if문 대신 다른 제어문 사용 권장
				if (uv_min.x <= uv.x && uv.x <= uv_max.x
					&& uv_min.y <= uv.y && uv.y <= uv_max.y)
				{
					ret = tex2D(_MainTex, uv).a;
				}
				
				return ret;
			}

			// circle로 검색해서 해당 픽셀 주변에 alpha값이 있는지 검색하여
			// alpha값이 있으면 색을 입력한다 
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

				// TOOD @sangmoon 더해진 alpha 총 값을 이용 + smoothstep을 이용하여 아웃라인이 자연스럽게 빠지도록 하자 				
				circleAlpha = clamp(circleAlpha, 0.0, 1.0);
				
				return float4(_OutlineColor.rgb, circleAlpha);
			}

			ENDCG
		}

		Pass{
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]{
			Combine texture * primary
		}
		}
	}
}