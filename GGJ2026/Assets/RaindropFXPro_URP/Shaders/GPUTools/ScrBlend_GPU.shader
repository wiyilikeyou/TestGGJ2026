Shader "Custom/RaindropFX/ScreenBlendEffect_GPU" {
	Properties{
		_MainTex("Base", 2D) = "" {}
	}

	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_AdditionalColTex);
	TEXTURE2D_X(_HeightMap);

	vector _MainTex_TexelSize;
	float _ColBlendAmount;
	float _Distortion;
	float _cutEdge;

	float _inBlack;
	float _inWhite;
	float _outWhite;
	float _outBlack;

	// color level
	float GetPixelLevel(float inPixel) {
		return (((inPixel * 255.0) - _inBlack) / (_inWhite - _inBlack) * (_outWhite - _outBlack) + _outBlack) / 255.0;
	}

	float4 HeightToNormal(float2 nowPos) {
		nowPos.x -= 1.0 * _MainTex_TexelSize.x;
		float4 leftColor = SAMPLE_TEXTURE2D_X(_HeightMap, sampler_LinearClamp, nowPos);
		float xLeft = GetPixelLevel(max(leftColor.r, max(leftColor.g, leftColor.b))) + 0.2;
		if (xLeft < _cutEdge) xLeft = pow(xLeft, 8.0);

		nowPos.x += 2.0 * _MainTex_TexelSize.x;
		float4 rightColor = SAMPLE_TEXTURE2D_X(_HeightMap, sampler_LinearClamp, nowPos);
		float xRight = GetPixelLevel(max(rightColor.r, max(rightColor.g, rightColor.b))) + 0.2;
		if (xRight < _cutEdge) xRight = pow(xRight, 8.0);

		nowPos.x -= 1.0 * _MainTex_TexelSize.x;
		nowPos.y += 1.0 * _MainTex_TexelSize.y;
		float4 upColor = SAMPLE_TEXTURE2D_X(_HeightMap, sampler_LinearClamp, nowPos);
		float yUp = GetPixelLevel(max(upColor.r, max(upColor.g, upColor.b))) + 0.2;
		if (yUp < _cutEdge) yUp = pow(yUp, 8.0);

		nowPos.y -= 2.0 * _MainTex_TexelSize.y;
		float4 downColor = SAMPLE_TEXTURE2D_X(_HeightMap, sampler_LinearClamp, nowPos);
		float yDown = GetPixelLevel(max(downColor.r, max(downColor.g, downColor.b))) + 0.2;
		if (yDown < _cutEdge) yDown = pow(yDown, 8.0);

		float xDelta = ((xLeft - xRight) + 1.0) * 0.5;
		float yDelta = ((yUp - yDown) + 1.0) * 0.5;

		return float4(saturate(xDelta), saturate(yDelta), 1.0f, 1.0f);
	}

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv    : TEXCOORD0;
	};

	v2f Vertf(Attributes input) {
		v2f output;
		output.pos = TransformObjectToHClip(input.positionOS.xyz);
		output.uv = input.uv;
		return output;
	}

	float4 Fragf(v2f i) : SV_Target{
		float4 mainColor = HeightToNormal(i.uv);

		float2 bump = (mainColor * 2 - 1).rg;
		mainColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv + bump * _Distortion);

		return mainColor;
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