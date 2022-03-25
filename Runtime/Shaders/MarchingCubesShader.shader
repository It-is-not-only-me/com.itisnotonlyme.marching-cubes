Shader "MarchingCubes/Render"
{
    Properties
    {
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"

            struct Triangulo {
                float3 vertexA;
                float3 vertexB;
                float3 vertexC;

                float3 normal;
            };

            StructuredBuffer<Triangulo> triangulos;

            struct v2g
            {
                float4 vertexA : SV_Position;
                float4 vertexB : TEXCOORD0;
                float4 vertexC : TEXCOORD1;
                float3 normal  : TEXCOORD2;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float2 normal : TEXCOORD0;
            };

            v2g vert (uint id : SV_VertexID)
            {
                Triangulo triangulo = triangulos[id];

                v2g o;
                o.vertexA = float4(triangulo.vertexA, 1);
                o.vertexB = float4(triangulo.vertexB, 1);
                o.vertexC = float4(triangulo.vertexC, 1);
                o.normal = triangulo.normal;
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> stream)
            {
                v2g triangulo = patch[0];

                g2f vertice;
                vertice.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice.normal = triangulo.normal;
                stream.Append(vertice);

                vertice.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice.normal = triangulo.normal;
                stream.Append(vertice);

                vertice.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice.normal = triangulo.normal;
                stream.Append(vertice);
            }

            float4 frag(g2f i) : SV_Target
            {
                return float4(1, 1, 1, 1);
            }

            ENDCG
        }
    }
}
