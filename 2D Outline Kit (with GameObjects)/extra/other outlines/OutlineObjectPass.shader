// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outline/OutlineObjectPass" {
	SubShader {
        Pass {
        	Blend Off
        	Lighting Off
        	AlphaTest Off
        
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            
            sampler2D _CameraDepthNormalsTexture;
            
            half4 weights;

			struct vertOut {
	            float4 pos:SV_POSITION;
	            float4 screenPos:TEXCOORD1;
	        };

            vertOut vert(appdata_base v) : POSITION {
            	vertOut o;
            	o.pos = UnityObjectToClipPos(v.vertex);
            	o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            half4 frag(vertOut i) : COLOR {

            	i.screenPos.xyz /= i.screenPos.w;
				#ifdef SHADER_API_D3D11
				float fragDepth = i.screenPos.z;
				#else
            	float fragDepth = (i.screenPos.z + 1) / 2;
				#endif

      			float3 normalValues;
				float depthValue;
				float4 x = tex2D(_CameraDepthNormalsTexture, i.screenPos.xy);
				DecodeDepthNormal(x, depthValue, normalValues);
      			
      			if(depthValue < (Linear01Depth(fragDepth) - 0.0005))
      			{
      				return half4(0.0, 0.0, 0.0, 0.0);
      			}
      			
                return weights;
            }

            ENDCG
        }
    }
}
