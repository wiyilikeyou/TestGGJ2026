Shader "Hidden/Custom/ScreenBlend" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_RFX_CullMask("CullMask", 2D) = "white" {}
		_RFX_WipeMap("WipeMap", 2D) = "black" {}
	}

	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_RFX_WetTex);
	TEXTURE2D_X(_RFX_CullMask);
	TEXTURE2D_X(_RFX_WipeMap);
	TEXTURE2D_X(_RFX_HeightMap);
	TEXTURE2D_X(_RFX_BlurOpaque);

	float4 _RainTex_TexelSize;
	float4 _FogTint;
	float4 _TintColor;

	int _IsUseFog;
	int _TintWeight;
	int _IsEnableWip;
	int _FogIteration;

	float _PixelSize;
	float _FogIntensity;
	float _Distortion;

	struct v2f {
		float4 posCS : SV_POSITION;
		float2 uv    : TEXCOORD0;
	};

	v2f Vertf(Attributes input) {
		v2f output;
		output.posCS = TransformObjectToHClip(input.positionOS.xyz);
		output.uv = input.uv;
		return output;
	}

	float GrayScale(float4 color) {
		return (color.r + color.g + color.b) / 3.0;
	}

	float4 Fragf(v2f i) : SV_Target {
		float4 mainColor;
		float2 nowPos = float2(i.uv.x, i.uv.y);
		if (_PixelSize >= 0) {
			float ratioX = (int)(i.uv.x * _PixelSize) / _PixelSize;
			float ratioY = (int)(i.uv.y * _PixelSize) / _PixelSize;
			nowPos = float2(ratioX, ratioY);
		}

		nowPos.x -= _RainTex_TexelSize.x;
		float4 leftColor = SAMPLE_TEXTURE2D_X(_RFX_HeightMap, sampler_LinearClamp, nowPos);
		float xLeft = GrayScale(leftColor);

		nowPos.x += 2.0 * _RainTex_TexelSize.x;
		float4 rightColor = SAMPLE_TEXTURE2D_X(_RFX_HeightMap, sampler_LinearClamp, nowPos);
		float xRight = GrayScale(rightColor);

		nowPos.x -= _RainTex_TexelSize.x;
		nowPos.y += _RainTex_TexelSize.y;
		float4 upColor = SAMPLE_TEXTURE2D_X(_RFX_HeightMap, sampler_LinearClamp, nowPos);
		float yUp = GrayScale(upColor.r);

		nowPos.y -= 2.0 * _RainTex_TexelSize.y;
		float4 downColor = SAMPLE_TEXTURE2D_X(_RFX_HeightMap, sampler_LinearClamp, nowPos);
		float yDown = GrayScale(downColor);

		float xDelta = (xLeft - xRight + 1.0) * 0.5;
		float yDelta = (yUp - yDown + 1.0) * 0.5;

		mainColor = float4(clamp(xDelta, 0.0, 1.0), clamp(yDelta, 0.0, 1.0), 1.0f, 1.0f);
		float2 bump = (mainColor * 2 - 1).rg;
		
		//mainColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + bump * _Distortion);
		mainColor = SAMPLE_TEXTURE2D_X(_RFX_BlurOpaque, sampler_LinearClamp, i.uv + bump * _Distortion);

		// tint
		float tintWeight = PositivePow(
			SAMPLE_TEXTURE2D_X(_RFX_HeightMap, sampler_LinearClamp, i.uv).r, 
			1.0 / _TintWeight
		);
		mainColor = lerp(mainColor, mainColor * _TintColor, tintWeight);

		if (_IsUseFog) {
			float wipeMask = SAMPLE_TEXTURE2D_X(_RFX_WipeMap, sampler_LinearClamp, i.uv).r;
			float4 wetMask = SAMPLE_TEXTURE2D_X(_RFX_WetTex, sampler_LinearClamp, i.uv);
			float4 wet = saturate(pow(wetMask + (_IsEnableWip ? wipeMask : 0), 0.5) * _FogIteration);
			mainColor = lerp(_FogTint * _FogIntensity + mainColor * 0.5, mainColor, wet);
		}

		float  cullMask = SAMPLE_TEXTURE2D_X(_RFX_CullMask, sampler_LinearClamp, i.uv).r;
		//float4 originOpaque = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
		float4 originOpaque = SAMPLE_TEXTURE2D_X(_RFX_BlurOpaque, sampler_LinearClamp, i.uv);
		return lerp(originOpaque, mainColor, cullMask);
	}
	ENDHLSL

	SubShader {
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		LOD 100
		ZTest Always 
		ZWrite Off 
		Cull Off

		Pass {
			Name "RAIN"
			HLSLPROGRAM
				#pragma vertex Vertf
				#pragma fragment Fragf
			ENDHLSL
		}
	}
}
