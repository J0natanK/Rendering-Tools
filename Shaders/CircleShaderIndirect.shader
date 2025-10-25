Shader "Unlit/CircleShaderIndirect"
{
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	    LOD 100
	    
	    ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct vertData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragData
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : TEXCOORD1;
            };

            StructuredBuffer<float3> vertices;
            StructuredBuffer<float2> uvs;
            StructuredBuffer<float4x4> objectToWorldBuffer;
            StructuredBuffer<float4> colors;

            fragData vert (uint svVertexID: SV_VertexID, uint svInstanceID : SV_InstanceID, vertData v)
            {
                InitIndirectDrawArgs(0);
                
                fragData data;
                
                uint instanceID = GetIndirectInstanceID(svInstanceID);
                uint vertexID = GetIndirectVertexID(svVertexID);
                
                float3 pos = vertices[vertexID];
                float4 wpos = mul(objectToWorldBuffer[instanceID], float4(pos, 1.0f));
                data.vertex = mul(UNITY_MATRIX_VP, wpos);
                data.color = colors[instanceID];
                data.uv = uvs[vertexID];
    
                return data;
            }

            fixed4 frag (fragData i) : SV_Target
            {
                float magnitude = length(i.uv - float2(0.5, 0.5));
                
                if(magnitude > 0.5)
                {
                    return float4(0, 0, 0, 0);
                }
                
                return i.color;
            }
            ENDCG
        }
    }
}
