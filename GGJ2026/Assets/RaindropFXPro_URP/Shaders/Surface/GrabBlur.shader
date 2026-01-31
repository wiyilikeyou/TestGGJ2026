Shader "Custom/RaindropFX/WetSurfaceURP" {
	Properties{
		[Header(_Color_)] [Space(5)]
		_DropTintItr("Drop Col Iter", Range(0,10)) = 1
		_DropTint("Droplet Color", Color) = (1, 1, 1, 1)
		_TintAmt("Tint Amount", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex("Tint Color (RGB)", 2D) = "white" {}

		[Header(_Distortion_)][Space(5)]
		_BumpAmt("Rain Distortion", range(0,32)) = 10
		_IOR("IOR", range(1.0,1.33)) = 1.1

		[Header(_Reflection_)][Space(5)]
		_Reflect("Reflect", Range(0,1)) = 0.3
		[NoScaleOffset] _Cube("Environment (Cube)", Cube) = "_Skybox"{}

		[Header(_Refraction_)][Space(5)]
		_Roughness("Roughness", Range(0,1)) = 1.0
		_RoughIter("Rough Iteration", Range(0.01,4)) = 0.2
		[NoScaleOffset] _RoughTex("Rough Map (Grayscale)", 2D) = "white" {}

		[Header(_Fog_)][Space(5)]
		_FogAmt("Fog", Range(0,1)) = 0
		_FogItr("Fog Iteration", Range(0,10)) = 1
		_FogCol("Fog Color", Color) = (1, 1, 1, 1)

		//[Header(_Textures_)] [Space(5)]
		[HideInInspector]
		_BumpMap("Normal Map (AUTO)", 2D) = "bump" {}
		[HideInInspector]
		_FogMaskMap("Wet Map (AUTO)", 2D) = "white" {}
		[HideInInspector]
		_WipeMap("Wipe Map (AUTO)", 2D) = "white" {}
	}

	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"

	TEXTURE2D_X(_MainTex);
	TEXTURE2D_X(_RoughTex);
	TEXTURE2D_X(_WipeMap);
	TEXTURE2D_X(_BumpMap);
	TEXTURE2D_X(_FogMaskMap);
	TEXTURE2D_X(_RFX_BlurOpaque);
	TEXTURE2D_X(_RFX_Opaque);
	TEXTURECUBE(_Cube);

	CBUFFER_START(UnityPerMaterial)
		float _IsUseWipe;
		float _FogAmt;
		float _FogItr;
		float _Reflect;
		float _Roughness;
		float _RoughIter;
		float _BumpAmt;
		//float _BumpDetailAmt;
		float _IOR;
		float _TintAmt;
		float4 _BumpMap_ST;
		float4 _MainTex_ST;
		float4 _FogCol;

		float4 _GrabTexture_TexelSize;
		float4 _DropTint;
		float _DropTintItr;
	CBUFFER_END

	struct vertAttr {
		float4 positionOS : POSITION;
		float3 normalOS   : NORMAL;
		float2 uv         : TEXCOORD0;
	};

	struct v2f {
		float3 reflex : NORMAL;
		float4 vertex : SV_POSITION;
		float4 uvgrab : TEXCOORD0;
		float2 uvbump : TEXCOORD1;
		float2 uvmain : TEXCOORD2;
	};

	v2f Vertf(vertAttr v) {
		v2f o;
		o.vertex = TransformObjectToHClip(v.positionOS.xyz);
		VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS.xyz);

		//o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y) + o.vertex.w) * 0.5;
		//o.uvgrab.zw = o.vertex.zw;
		o.uvgrab = ComputeScreenPos(o.vertex);
		o.uvbump = TRANSFORM_TEX(v.uv, _BumpMap);
		o.uvmain = TRANSFORM_TEX(v.uv, _MainTex);

		float3 worldViewDir = GetWorldSpaceViewDir(o.vertex.xyz);
		o.reflex = reflect(-worldViewDir, normalInputs.normalWS);
		return o;
	}

	float4 Fragf(v2f i) : SV_Target {
		float4 tint = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uvmain);
		float fogMask = SAMPLE_TEXTURE2D_X(_FogMaskMap, sampler_LinearClamp, i.uvmain).r;
		float roughMask = SAMPLE_TEXTURE2D_X(_RoughTex, sampler_LinearRepeat, i.uvmain).r;
		float wipeMask = 1.0 - (_IsUseWipe > 0 ? SAMPLE_TEXTURE2D_X(_WipeMap, sampler_LinearClamp, i.uvmain).r : 0.0);
		float3 bump = UnpackNormal(SAMPLE_TEXTURE2D_X(_BumpMap, sampler_LinearClamp, i.uvbump)) * fogMask;
		float tintWeight = PositivePow(
			saturate(
				PositivePow(
					fogMask * (_DropTintItr * 0.3 + 1.0), 
					_DropTintItr + 1.0
				)
			), 1.0 / _DropTintItr
		);
		float2 offset = bump.rg * _BumpAmt * 10.0;

		float ior = (_IOR - 1.0) / 40.0;
		float2 distortedUV = (offset + i.uvgrab.xy) / i.uvgrab.w + float2(ior, ior);
		float4 col = SAMPLE_TEXTURE2D_X(_RFX_BlurOpaque, sampler_LinearClamp, distortedUV);
		float4 opaqueCol = SAMPLE_TEXTURE2D_X(_RFX_Opaque, sampler_LinearClamp, distortedUV);
		col = lerp(
			lerp(opaqueCol, col, _Roughness), 
			opaqueCol, 
			saturate(PositivePow(fogMask * 4.0, _RoughIter))
		);
		col = lerp(opaqueCol, col, roughMask * wipeMask);

		float3 refUV = float3(i.reflex.x, 1.0 - i.reflex.y, i.reflex.z) + bump * clamp(fogMask + 0.2, 0, 1) * _BumpAmt;
		float4 ref = SAMPLE_TEXTURECUBE_LOD(_Cube, sampler_LinearClamp, refUV, roughMask * _Roughness * 8.0);
		float4 fcol = lerp(col, ref, _Reflect);
		fcol = lerp(fcol, tint, _TintAmt);
		col = lerp(col, tint, _TintAmt);

		float4 wet = clamp(pow(fogMask, 0.5) * _FogItr, 0, 1);
		col = lerp(col, col * wet + (_FogCol + col * 0.5) * (1.0 - wet), _FogAmt * wipeMask);
		col = lerp(col, ref, _Reflect * clamp(wet * wet, 0, 1));
		col = lerp(col, fcol, 1.0 - clamp(_FogAmt * 5, 0, 1));
		col = lerp(col, col * _DropTint, tintWeight);
		return col;
	}
	ENDHLSL

	SubShader {
		Tags{ 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
		}
		LOD 100
		ZTest LEqual
		ZWrite On
		Cull Back

		Pass {
			Name "WetGlass"
			HLSLPROGRAM
				#pragma vertex Vertf
				#pragma fragment Fragf
			ENDHLSL
		}
	}
}