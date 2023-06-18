Shader "Custom/UIBackground"
{
    Properties
    {
        _MainText("Texture", 2D) = "white" {}
        _BottomLeftColour("Top left colour", Color) = (0,0,0,0)
        _TopRightColour("Bottom right colour", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        HLSLINCLUDE

        #pragma target 5.0
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        uniform float4 _BottomLeftColour;
        uniform float4 _TopRightColour;

        ENDHLSL
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };

            Interpolators vert (Attributes i)
            {
                Interpolators o;
                o.positionHCS = TransformObjectToHClip(i.positionOS);
                o.uv = i.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                return lerp(_BottomLeftColour, _TopRightColour, i.uv.x);
            }
            ENDHLSL
        }
    }
}
