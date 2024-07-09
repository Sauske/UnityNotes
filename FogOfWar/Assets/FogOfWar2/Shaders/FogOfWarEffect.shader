Shader "FOW/FogOfWarEffect"
{
    Properties
    {
        _FogColor("FogColor", color) = (0,0,0,1)
        _MixValue("MixValue", float) = 0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_depth : TEXCOORD1;
                float3 interpolatedRay : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)

            half _MixValue;

            half4 _FogColor;

            CBUFFER_END

            float4x4 internal_WorldToProjector;

            float4x4 _FrustumCornersRay;

            float4 _CameraColorTexture_TexelSize;

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_FogTex);
            SAMPLER(sampler_FogTex);

            v2f vert(Attributes v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                o.uv_depth = v.uv;

                #if UNITY_UV_STARTS_AT_TOP
                if (_CameraColorTexture_TexelSize.y < 0)
                    o.uv_depth.y = 1 - o.uv_depth.y;
                #endif

                //根据纹理坐标特性判定属于哪个角
                int index;
                if (v.uv.x < 0.5 && v.uv.y < 0.5)
                {
                    index = 0;
                }
                else if (v.uv.x > 0.5 && v.uv.y < 0.5)
                {
                    index = 1;
                }
                else if (v.uv.x > 0.5 && v.uv.y > 0.5)
                {
                    index = 2;
                }
                else
                {
                    index = 3;
                }

                #if UNITY_UV_STARTS_AT_TOP
                if (_CameraColorTexture_TexelSize.y < 0)
                    index = 3 - index;
                #endif

                o.interpolatedRay = _FrustumCornersRay[index].xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 c = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, i.uv);

                // 先对深度纹理进行采样，再得到视角空间下的线性深度值
                float linearDepth = LinearEyeDepth(
                    SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv_depth), _ZBufferParams);

                // 得到世界空间下的位置
                float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.interpolatedRay.xyz;
                // 通过internal_CameraToProjector矩阵最终得到战争迷雾uv空间坐标
                worldPos = mul(internal_WorldToProjector, float4(worldPos.xyz, 1));
                // 使用战争迷雾uv空间坐标对迷雾纹理进行采样
                float3 tex = SAMPLE_TEXTURE2D(_FogTex, sampler_FogTex, worldPos.xz).rgb;

                float2 atten = saturate((0.5 - abs(worldPos.xz - 0.5)) / (1 - 0.9));

                float3 col;
                col.rgb = lerp(_FogColor.rgb, float3(1, 1, 1), tex.r * _FogColor.a);

                float visual = lerp(tex.b, tex.g, _MixValue);
                col.rgb = lerp(col.rgb, float3(1, 1, 1), visual) * atten.x * atten.y;

                c.rgb *= col.rgb;
                c.a = 1;
                return c;
            }
            ENDHLSL
        }
    }
}