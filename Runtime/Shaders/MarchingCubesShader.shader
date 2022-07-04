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

            struct Triangle {
                float3 vertexA;
                float3 vertexB;
                float3 vertexC;
                float2 uvA;
                float2 uvB;
                float2 uvC;
            };

            StructuredBuffer<Triangle> triangulos;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2g
            {
                float4 vertexA : TEXCOORD0;
                float4 vertexB : TEXCOORD1;
                float4 vertexC : TEXCOORD2;
                float3 normal  : TEXCOORD3;
                float2 uvA : TEXCOORD4;
                float2 uvB : TEXCOORD5;
                float2 uvC : TEXCOORD6;
            };

            struct g2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2g vert (uint id : SV_VertexID)
            {
                Triangle triangulo = triangulos[id];

                v2g o;
                o.vertexA = float4(triangulo.vertexA, 1);
                o.uvA = triangulo.uvA;
                o.vertexB = float4(triangulo.vertexB, 1);
                o.uvB = triangulo.uvB;
                o.vertexC = float4(triangulo.vertexC, 1);
                o.uvC = triangulo.uvC;
                o.normal = -normalize(cross(triangulo.vertexB - triangulo.vertexA, triangulo.vertexC - triangulo.vertexA));
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> triStream)
            {
                v2g triangulo = patch[0];

                g2f vertice1;
                vertice1.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice1.normal = triangulo.normal;
                vertice1.uv = triangulo.uvC;
                triStream.Append(vertice1);

                g2f vertice2;
                vertice2.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice2.normal = triangulo.normal;
                vertice2.uv = triangulo.uvB;
                triStream.Append(vertice2);

                g2f vertice3;
                vertice3.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice3.normal = triangulo.normal;
                vertice3.uv = triangulo.uvA;
                triStream.Append(vertice3);

                triStream.RestartStrip();
            }

            float4 frag(g2f i) : SV_Target
            {
                float3 color = tex2D(_MainTex, 0);
                float orientacion = saturate(dot(_WorldSpaceLightPos0.xyz, i.normal));

                return float4(color * orientacion, 1);
                //return float4(i.normal, 1);
            }

            ENDCG
        }
    }
}
