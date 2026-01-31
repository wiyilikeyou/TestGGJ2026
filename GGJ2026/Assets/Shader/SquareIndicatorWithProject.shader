Shader "Custom/SquareIndicatorWithProject"
{
    Properties
    {
        // 基础指示器颜色
        _MainColor ("指示器主色", Color) = (1,0.8,0,1)
        // 发光强度（自发光，让指示器更醒目）
        _Emission ("发光强度", Range(0, 5)) = 1
        // 指示器大小（单位：世界空间米）
        _IndicatorSize ("方形大小", Float) = 2
        // 方形圆角（0=直角，1=最大圆角）
        _CornerSmooth ("圆角平滑度", Range(0,1)) = 0.1
        // 显示模式：0=实心填充 1=仅轮廓
        _DisplayMode ("显示模式(0=填充|1=轮廓)", Float) = 0
        // 轮廓宽度（仅模式1生效）
        _OutlineWidth ("轮廓宽度", Range(0.01, 0.5)) = 0.1

        // 投影相关
        _ProjectColor ("投影颜色", Color) = (0,0,0,0.3)
        // 投影模糊度（柔边效果）
        _ProjectBlur ("投影模糊度", Range(0.01, 0.5)) = 0.1
        // 投影与指示器的偏移（向上/向下微调，避免穿模）
        _ProjectOffset ("投影Y轴偏移", Float) = 0.01
        // 投影大小缩放（比指示器大一点，更自然）
        _ProjectScale ("投影缩放", Float) = 1.2

        // 通用限制
        // 最大显示距离（相机到指示器超过该值则隐藏）
        _MaxViewDistance ("最大显示距离", Float) = 50
    }

    SubShader
    {
        // 渲染队列：透明队列，保证投影/指示器在地面之上、模型之下
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        // 关闭深度写入，开启混合（透明效果）
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        // 双面渲染，避免视角问题导致指示器消失
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 声明属性变量
            CBUFFER_START(UnityPerMaterial)
            float4 _MainColor;
            float _Emission;
            float _IndicatorSize;
            float _CornerSmooth;
            float _DisplayMode;
            float _OutlineWidth;
            float4 _ProjectColor;
            float _ProjectBlur;
            float _ProjectOffset;
            float _ProjectScale;
            float _MaxViewDistance;
            CBUFFER_END

            // 顶点输入结构
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            // 顶点输出结构
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float viewDistance : TEXCOORD2;
                // 投影的屏幕位置
                float4 projectPos : TEXCOORD3;
            };

            // 计算圆角方形的SDF（有符号距离场，核心算法）
            float GetSquareSDF(float2 uv, float size, float corner)
            {
                uv = uv * 2 - 1; // UV归一化到[-1,1]
                uv *= size;
                // 计算圆角
                float2 box = float2(size - corner, size - corner);
                float2 d = abs(uv) - box;
                return min(max(d.x, d.y), 0.0) + length(max(d, 0.0)) - corner;
            }

            v2f vert (appdata v)
            {
                v2f o;
                // 模型空间转世界空间
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldPos = worldPos;
                // 计算相机到指示器的距离（用于距离限制）
                o.viewDistance = distance(_WorldSpaceCameraPos, worldPos);
                // 模型空间转裁剪空间（屏幕显示）
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;

                // 计算投影的世界位置：Y轴偏移，大小缩放
                float3 projectWorldPos = worldPos;
                projectWorldPos.y += _ProjectOffset;
                projectWorldPos.xy = (worldPos.xy - _WorldSpaceCameraPos.xy) * _ProjectScale + _WorldSpaceCameraPos.xy;
                // 投影位置转裁剪空间
                o.projectPos = TransformWorldToHClip(projectWorldPos);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 超过最大显示距离，直接透明隐藏
                if(i.viewDistance > _MaxViewDistance) discard;

                half4 col = half4(0,0,0,0);
                float2 uv = i.uv;
                // 计算指示器的SDF距离
                float sdf = GetSquareSDF(uv, _IndicatorSize * 0.5, _CornerSmooth * _IndicatorSize * 0.5);
                // 计算投影的SDF距离（带模糊）
                float projectSdf = GetSquareSDF(uv, _IndicatorSize * 0.5 * _ProjectScale, (_CornerSmooth + _ProjectBlur) * _IndicatorSize * 0.5);

                // 1. 绘制投影（柔边半透）
                float projectAlpha = smoothstep(_ProjectBlur, 0, abs(projectSdf));
                col += _ProjectColor * projectAlpha;

                // 2. 绘制方形指示器
                float indicatorAlpha = 0;
                if(_DisplayMode < 0.5) // 0=实心填充模式
                {
                    indicatorAlpha = smoothstep(0, 0.01, -sdf);
                }
                else // 1=轮廓模式
                {
                    float outlineInner = _OutlineWidth * _IndicatorSize;
                    indicatorAlpha = smoothstep(outlineInner, 0, abs(sdf)) - smoothstep(0, 0.01, abs(sdf) - outlineInner);
                }
                // 叠加指示器颜色+发光
                half4 indicatorCol = _MainColor * indicatorAlpha * (1 + _Emission);
                indicatorCol.rgb *= _Emission;
                col += indicatorCol;

                // 限制总透明度，避免过亮
                col.a = saturate(col.a);
                return col;
            }
            ENDHLSL
        }
    }
    // 回退到内置管线的透明着色器（兼容低版本）
    FallBack "Transparent/Unlit"
}