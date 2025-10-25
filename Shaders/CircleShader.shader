Shader "Unlit/CircleShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
    }

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

            struct vertData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragData
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            fragData vert (vertData v)
            {
                fragData o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 _Color;

            fixed4 frag (fragData i) : SV_Target
            {
                float magnitude = length(i.uv - float2(0.5, 0.5));
                
                if(magnitude > 0.5)
                {
                    return float4(0, 0, 0, 0);
                }
                
                return _Color;
            }
            ENDCG
        }
    }
}
