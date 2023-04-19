// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "TestShader/Shader-1"
{
    Properties
    {
        _Tint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Tint;

            float4 vert(float4 position: POSITION, out float3 localPos: TEXCOORD0) : SV_POSITION
            {
                localPos = position.xyz;
                return UnityObjectToClipPos(position);
            }
            float4 frag (float4 position: SV_POSITION, float3 localPos: TEXCOORD0) : SV_Target
            {
                return float4(localPos, 1);
            }
            ENDCG
        }
    }
}
