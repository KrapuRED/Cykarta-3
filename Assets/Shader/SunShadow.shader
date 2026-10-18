Shader "Custom/SunShadow"
{
    Properties { _Color ("Color", Color) = (0,0,0,0.45) }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Stencil { Ref 1 Comp NotEqual Pass Replace }

        Pass
        {
            Tags { "LightMode"="SRPDefaultUnlit" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            half4 _Color;
            struct A { float4 pos : POSITION; };
            struct V { float4 pos : SV_POSITION; };

            V vert (A a) { V v; v.pos = TransformObjectToHClip(a.pos.xyz); return v; }
            half4 frag (V v) : SV_Target { return _Color; }
            ENDHLSL
        }
    }
}