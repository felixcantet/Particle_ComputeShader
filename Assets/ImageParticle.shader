Shader "Custom/ImageParticle"
{
    Properties
    {
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent"  "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
         ColorMask RGB
     Cull Off Lighting Off ZWrite Off

         LOD 100

         Pass
         {
             CGPROGRAM

             #pragma vertex vert
             #pragma fragment frag

             #include "UnityCG.cginc"

             
             struct Particle
             {
                float4 color;
                float3 position;
                float3 initialPosition;
                float3 velocity;
                float3 targetPosition;
                float interpolationFactor;
                bool interpolationGrow;
                bool move;
                float3 startMovePosition;
                float4 initialColor;
             };


             struct v2f
             {
                 float4 color : COLOR;
                 float4 position : SV_POSITION;
             };

             StructuredBuffer<Particle> _Buffer;
             
             v2f vert(uint instance_ID : SV_VertexID)
             {
                 v2f o;
                 o.position = UnityObjectToClipPos(float4(_Buffer[instance_ID].position, 1.0));
                 o.color = _Buffer[instance_ID].color;
                 return o;
             }


             fixed4 frag(v2f i) : SV_Target
             {
                 float4 col = i.color;
                 return col;
             }
             ENDCG
                     }
    }
}
