Shader "Custom/Glow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius("Radius", float) = 2.0
        _GlowRadius("Glow Radius", float) = 2.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        Blend One OneMinusSrcAlpha
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            #pragma target 5.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Radius;
            float _GlowRadius;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 positionHCS : SV_POSITION;
                float4 colour : COLOR;
                float2 uv : TEXCOORD0;
                float4 originWS : TEXCOORD1;
                float4 positionWS : TEXCOORD2; 
            };


            Interpolators vert (Attributes i)
            {
                Interpolators o;
                o.originWS = mul(unity_ObjectToWorld, float4(0,0,0,1));
                o.positionWS = mul(unity_ObjectToWorld, i.positionOS);
                o.positionHCS = TransformObjectToHClip(i.positionOS);
                o.uv = i.uv;
                return o;
            }

            float4 frag (Interpolators i) : SV_TARGET0
            {
                float4 baseTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float dist = distance(i.originWS, i.positionWS);
                float4 alpha = (dist <= _GlowRadius) ? 1 : 0.0;
                clip( alpha <= 0.0 ? -1:1 ); //discard pixel if alpha of pixel is zero
                baseTex.a =  1 - smoothstep(_Radius, _GlowRadius, dist);
                return baseTex;
                //return baseTex;
            }
            ENDHLSL
        }
    }
}
