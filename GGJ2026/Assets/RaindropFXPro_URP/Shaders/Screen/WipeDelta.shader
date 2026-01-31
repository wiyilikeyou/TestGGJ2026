Shader "Hidden/Custom/Wiper" {
    Properties {
		_MainTex("Base", 2D) = "" {}
		_WipeTex("Wipe", 2D) = "black" {}
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	sampler2D _WipeTex;

	v2f vert(appdata_img v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	}

	half4 frag(v2f i) : COLOR {
		float4 mainColor = tex2D(_MainTex, i.uv);
		float alpha = 1.0 - pow(tex2D(_WipeTex, i.uv).r, 8.0);

		mainColor.rgb *= alpha;
		return mainColor;
	}

	ENDCG

	Subshader {
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog {
				Mode off
			}

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}
