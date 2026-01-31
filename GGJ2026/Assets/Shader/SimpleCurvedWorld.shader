Shader "Custom/SimpleCurvedWorld_URP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CurveX ("Curve X", Float) = 0
        _CurveY ("Curve Y", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 引入 URP 核心库
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _CurveX;
                float _CurveY;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 1. 获取顶点在 Object Space 的位置
                float3 positionOS = input.positionOS.xyz;

                // 2. 转换到 World Space (世界坐标)
                float3 worldPos = TransformObjectToWorld(positionOS);

                // 3. 计算距离 (相对于摄像机)
                float3 cameraPos = _WorldSpaceCameraPos;
                float dist = worldPos.z - cameraPos.z;

                // 4. 计算偏移 (只在相机前方弯曲)
                // 简单的防穿帮保护：如果 dist < 0 (在背后)，就不弯曲或者减少弯曲
                float offset = dist * dist;
                
                worldPos.x += offset * _CurveX;
                worldPos.y -= offset * _CurveY;

                // 5. 转换到 Clip Space (屏幕坐标)
                output.positionCS = TransformWorldToHClip(worldPos);
                
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                return col;
            }
            ENDHLSL
        }
    }
}