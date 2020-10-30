// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageParticle"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
         [HDR] _Color("Color", Color) = (1, 0, 0, 1)
        _Size("Size", Float) = 1
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
             // Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members uv)

                         #pragma vertex vert
                         #pragma fragment frag
                        // #pragma geometry geo

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

                         /*struct appdata
                         {
                             float4 vertex : POSITION;
                             float2 uv : TEXCOORD0;
                         };*/

                         struct v2f
                         {
                             float4 color : COLOR;
                             float4 position : SV_POSITION;
                           //  float life : LIFE;
                            // float2 uv : TEXCOORD0;
                         };

                         StructuredBuffer<Particle> _Buffer;
                         uniform float4 _Color;
                         // Put this in a Constant Buffer
                         uniform float _Size;
                         uniform sampler2D _MainTex;
                         v2f vert(uint instance_ID : SV_VertexID)
                         {
                             v2f o;
                             //o.position = mul(unity_ObjectToWorld, float4(_Buffer[instance_ID].position, 1.0));
                             o.position = UnityObjectToClipPos(float4(_Buffer[instance_ID].position, 1.0));
                             o.color = _Buffer[instance_ID].color;
                             return o;
                         }

                         //[maxvertexcount(4)]
                         //void geo(point GS_INPUT p[1], inout TriangleStream<v2f> triStream)
                         //{
                         //    float3 up = float3(0, 1, 0);
                         //    float3 look = _WorldSpaceCameraPos - p[0].position;
                         //    look.y = 0;
                         //    look = normalize(look);
                         //    float3 right = cross(up, look);
                         //    float halfS = 0.5f - _Size;

                         //    float4 v[4];
                         //    v[0] = float4(p[0].position + halfS * right - halfS * up, 1.0f);
                         //    v[1] = float4(p[0].position + halfS * right + halfS * up, 1.0f);
                         //    v[2] = float4(p[0].position - halfS * right - halfS * up, 1.0f);
                         //    v[3] = float4(p[0].position - halfS * right + halfS * up, 1.0f);
                         //    /*v[0] = float4(p[0].position + float3(_halfS, _halfS, 0 ), 1.0);
                         //    v[1] = float4(p[0].position + float3(-_halfS, _halfS, 0), 1.0);
                         //    v[2] = float4(p[0].position + float3(-_halfS, -_halfS, 0), 1.0);
                         //    v[3] = float4(p[0].position + float3(_halfS, -_halfS, 0), 1.0);*/

                         //    //float4x4 vp = UnityObjectToClipPos(unity_WorldToObject);
                         //    v2f o;

                         //    o.position = UnityObjectToClipPos(v[0]);
                         //    o.color = p[0].color;
                         //    o.life = p[0].life;
                         //    o.uv = float2(1.0, 0.0);
                         //    triStream.Append(o);

                         //    o.position = UnityObjectToClipPos(v[1]);
                         //    o.color = p[0].color;
                         //    o.life = p[0].life;
                         //    o.uv = float2(1.0, 1.0);
                         //    triStream.Append(o);

                         //    o.position = UnityObjectToClipPos(v[2]);
                         //    o.color = p[0].color;
                         //    o.life = p[0].life;
                         //    o.uv = float2(0.0, 0.0);
                         //    triStream.Append(o);

                         //    o.position = UnityObjectToClipPos(v[3]);
                         //    o.color = p[0].color;
                         //    o.life = p[0].life;
                         //    o.uv = float2(0.0, 1.0);
                         //    triStream.Append(o);
                         //}

                         fixed4 frag(v2f i) : SV_Target
                         {
                             //return i.color;
                             float4 col = i.color * _Color;
                             //col *= i.color;
                             return col;
                         }
                         ENDCG
                     }
    }
}
