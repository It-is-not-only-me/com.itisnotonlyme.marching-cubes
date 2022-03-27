Shader "MarchingCubes/Render"
{
    Properties
    {
    }

    SubShader
    {
        Tags { 
            "Queue" = "Geometry"
            "RenderType" = "Opaque" 
        }
        //Cull Off

        Pass
        {
            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Triangle {
                float3 vertexA;
                float3 vertexB;
                float3 vertexC;
            };

            StructuredBuffer<Triangle> triangulos;

            struct v2g
            {
                float4 vertexA : TEXCOORD0;
                float4 vertexB : TEXCOORD1;
                float4 vertexC : TEXCOORD2;
                float3 normal  : TEXCOORD3;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD3;
            };

            v2g vert (uint id : SV_VertexID)
            {
                Triangle triangulo = triangulos[id];

                v2g o;
                o.vertexA = float4(triangulo.vertexA, 1);
                o.vertexB = float4(triangulo.vertexB, 1);
                o.vertexC = float4(triangulo.vertexC, 1);
                o.normal = normalize(cross(triangulo.vertexB - triangulo.vertexA, triangulo.vertexC - triangulo.vertexA));
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> triStream)
            {
                v2g triangulo = patch[0];

                g2f vertice1;
                vertice1.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice1.normal = triangulo.normal;
                triStream.Append(vertice1);

                g2f vertice2;
                vertice2.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice2.normal = triangulo.normal;
                triStream.Append(vertice2);

                g2f vertice3;
                vertice3.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice3.normal = triangulo.normal;
                triStream.Append(vertice3);

                triStream.RestartStrip();
            }

            float4 frag(g2f i) : SV_Target
            {
                return float4(1, 1, 1, 1);
            }

            ENDCG
        }
    }
}
