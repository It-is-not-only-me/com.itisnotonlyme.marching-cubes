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

            struct Vertice {
                float3 vertex;
                float2 uv;
                float2 uv2;
                float3 color;
            };

            struct Triangle {
                Vertice verticeA;
                Vertice verticeB;
                Vertice verticeC;
            };

            StructuredBuffer<Triangle> triangulos;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2g
            {
                float4 vertexA : TEXCOORD0;
                float4 vertexB : TEXCOORD1;
                float4 vertexC : TEXCOORD2;
                float4 uvAB : TEXCOORD3;
                float4 uvC2A : TEXCOORD4;
                float4 uv2BC : TEXCOORD5;
                float4 colorAB : TEXCOORD6;
                float4 colorBC : TEXCOORD7;
                float4 colorCNormal : TEXCOORD8;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 color : TEXCOORD3;
            };

            v2g vert (uint id : SV_VertexID)
            {
                Triangle triangulo = triangulos[id];

                v2g o;

                o.vertexA = float4(triangulo.verticeA.vertex, 1);
                o.vertexB = float4(triangulo.verticeB.vertex, 1);
                o.vertexC = float4(triangulo.verticeC.vertex, 1);

                o.uvAB = float4(triangulo.verticeA.uv, triangulo.verticeB.uv);
                o.uvC2A = float4(triangulo.verticeC.uv, triangulo.verticeA.uv2);
                o.uv2BC = float4(triangulo.verticeB.uv2, triangulo.verticeC.uv2);

                o.colorAB = float4(triangulo.verticeA.color, triangulo.verticeB.color.r);
                o.colorBC = float4(triangulo.verticeB.color.gb, triangulo.verticeC.color.rg);
                
                float3 normal = -normalize(cross(o.vertexB.xyz - o.vertexA.xyz, o.vertexC.xyz - o.vertexA.xyz));
                o.colorCNormal = float4(triangulo.verticeC.color.b, normal);

                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> triStream)
            {
                v2g triangulo = patch[0];

                float3 normal = triangulo.colorCNormal.yzw;

                g2f vertice1;
                vertice1.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice1.normal = normal;
                vertice1.uv = triangulo.uvC2A.xy;
                vertice1.uv2 = triangulo.uv2BC.zw;
                vertice1.color = float3(triangulo.colorBC.zw, triangulo.colorCNormal.x);
                triStream.Append(vertice1);

                g2f vertice2;
                vertice2.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice2.normal = normal;
                vertice2.uv = triangulo.uvAB.zw;
                vertice2.uv2 = triangulo.uv2BC.xy;
                vertice2.color = float3(triangulo.colorAB.w, triangulo.colorBC.xy);
                triStream.Append(vertice2);

                g2f vertice3;
                vertice3.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice3.normal = normal;
                vertice3.uv = triangulo.uvAB.xy;
                vertice3.uv2 = triangulo.uvC2A.zw;
                vertice3.color = triangulo.colorAB.xyz;
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
