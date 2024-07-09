Shader "FOW/FogOfWarBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Offset ("Offset", float) = 0.003
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

        CBUFFER_START(UnityPerMaterial)

        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

        float _Offset;

        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
        ENDHLSL

        Pass
        {
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;

                return output;
            }

            real4 frag(Attributes i):COLOR
            {
                float4 uv01 = i.uv.xyxy + _Offset * float4(0, 1, 0, -1);
                float4 uv10 = i.uv.xyxy + _Offset * float4(1, 0, -1, 0);
                float4 uv23 = i.uv.xyxy + _Offset * float4(0, 1, 0, -1) * 2.0;
                float4 uv32 = i.uv.xyxy + _Offset * float4(1, 0, -1, 0) * 2.0;
                float4 uv45 = i.uv.xyxy + _Offset * float4(0, 1, 0, -1) * 3.0;
                float4 uv54 = i.uv.xyxy + _Offset * float4(1, 0, -1, 0) * 3.0;

                real4 c = real4(0, 0, 0, 0);
                c += 0.4 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                c += 0.075 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv01.xy);
                c += 0.075 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv01.zw);
                c += 0.075 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv10.xy);
                c += 0.075 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv10.zw);
                c += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv23.xy);
                c += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv23.zw);
                c += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv32.xy);
                c += 0.05 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv32.zw);
                c += 0.025 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv45.xy);
                c += 0.025 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv45.zw);
                c += 0.025 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv54.xy);
                c += 0.025 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv54.zw);

                return c;
            }
            ENDHLSL
        }
    }
    FallBack off
}