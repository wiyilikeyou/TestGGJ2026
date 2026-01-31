Shader "Custom/RaindropFX/Wiper" {
	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	struct vertAttr {
		float4 positionOS : POSITION;
	};

	struct v2f {
		float4 vertex : SV_POSITION;
	};

	v2f Vertf(vertAttr v) {
		v2f o; o.vertex = TransformObjectToHClip(v.positionOS.xyz);
		return o;
	}

	float4 Fragf(v2f i) : SV_Target {
		return float4(1.0, 1.0, 1.0, 1.0);
	}
	ENDHLSL

	SubShader {
		Tags{ 
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100
		ZTest LEqual
		ZWrite On
		Cull Back

		Pass {
			Name "Wiper"
			HLSLPROGRAM
				#pragma vertex Vertf
				#pragma fragment Fragf
			ENDHLSL
		}
	}
}