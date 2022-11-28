Shader "MarchingCubes/Render"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { 
            "Queue" = "Geometry"
            "RenderType" = "Opaque" 
        }

        Pass
        {
            CGPROGRAM
            #pragma target 5.0

            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct Vertice 
            {
                float3 vertex;
                float4 uvs;
            };

            struct Triangle 
            {
                Vertice verticeA;
                Vertice verticeB;
                Vertice verticeC;
            };

            struct Plano 
            {
                float3 normal;
                float distancia;
            };

            StructuredBuffer<Triangle> triangulos;
            StructuredBuffer<Plano> planos;

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2g
            {
                float4 vertexA : TEXCOORD0;
                float4 vertexB : TEXCOORD1;
                float4 vertexC : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float4 uvsA : TEXCOORD4;
                float4 uvsB : TEXCOORD5;
                float4 uvsC : TEXCOORD6;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float4 uvs : TEXCOORD1;
            };

            v2g vert (uint id : SV_VertexID)
            {
                Triangle triangulo = triangulos[id];
                v2g o;

                o.vertexA = float4(triangulo.verticeA.vertex, 1);
                o.uvsA = triangulo.verticeA.uvs;

                o.vertexB = float4(triangulo.verticeB.vertex, 1);
                o.uvsB = triangulo.verticeB.uvs;

                o.vertexC = float4(triangulo.verticeC.vertex, 1);
                o.uvsC = triangulo.verticeC.uvs;

                o.normal = -normalize(cross(o.vertexB.xyz - o.vertexA.xyz, o.vertexC.xyz - o.vertexA.xyz));
                
                return o;
            }

            bool DentroDeCamaraFrustum(float3 vertice)
            {
                bool adentro = true;
                
                for (int i = 0; i < 6; i++) 
                {
                    float3 normal = planos[i].normal;
                    float distancia = planos[i].distancia;

                    float3 puntoEnPlano = normal * -distancia;
                    float3 verticeTransladado = vertice - puntoEnPlano;

                    adentro = adentro && dot(normal, verticeTransladado) > 0;
                }

                return adentro;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> triStream)
            {
                v2g triangulo = patch[0];

                bool dentroVerticeA = DentroDeCamaraFrustum(triangulo.vertexA);
                bool dentroVerticeB = DentroDeCamaraFrustum(triangulo.vertexB);
                bool dentroVerticeC = DentroDeCamaraFrustum(triangulo.vertexC);

                if (!dentroVerticeA && !dentroVerticeB && !dentroVerticeC)
                    return;

                float3 normal = triangulo.normal;

                g2f vertice1;
                vertice1.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice1.normal = normal;
                vertice1.uvs = triangulo.uvsC;
                triStream.Append(vertice1);

                g2f vertice2;
                vertice2.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice2.normal = normal;
                vertice2.uvs = triangulo.uvsB;
                triStream.Append(vertice2);

                g2f vertice3;
                vertice3.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice3.normal = normal;
                vertice3.uvs = triangulo.uvsA;
                triStream.Append(vertice3);

                triStream.RestartStrip();
            }

            float4 frag(g2f i) : SV_Target
            {
                float3 color = tex2D(_MainTex, 0);
                float orientacion = saturate(dot(_WorldSpaceLightPos0.xyz, i.normal));

                return float4(color * orientacion, 1);
            }

            ENDCG
        }
    }
}
