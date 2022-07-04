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
                float2 uvA;
                float2 uv2A;
                float3 colorA;
                float3 vertexB;
                float2 uvB;
                float2 uv2B;
                float3 colorB;
                float3 vertexC;
                float2 uvC;
                float2 uv2C;
                float3 colorC;
            };

            StructuredBuffer<Triangle> triangulos;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2g
            {
                float4 vertexA : TEXCOORD0;
                float4 vertexB : TEXCOORD1;
                float4 vertexC : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float2 uvA : TEXCOORD4;
                float2 uvB : TEXCOORD5;
                float2 uvC : TEXCOORD6;
                float2 uv2A : TEXCOORD7;
                float2 uv2B : TEXCOORD8;
                float2 uv2C : TEXCOORD9;
                float3 colorA : TEXCOORD10;
                float3 colorB : TEXCOORD11;
                float3 colorC : TEXCOORD12;
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

                o.vertexA = float4(triangulo.vertexA, 1);
                o.uvA = triangulo.uvA;
                o.uv2A = triangulo.uv2A;
                o.colorA = triangulo.colorA;

                o.vertexB = float4(triangulo.vertexB, 1);
                o.uvB = triangulo.uvB;
                o.uv2B = triangulo.uv2B;
                o.colorB = triangulo.colorB;

                o.vertexC = float4(triangulo.vertexC, 1);
                o.uvC = triangulo.uvC;
                o.uv2C = triangulo.uv2C;
                o.colorC = triangulo.colorC;

                o.normal = -normalize(cross(o.vertexB.xyz - o.vertexA.xyz, o.vertexC.xyz - o.vertexA.xyz));
                
                return o;
            }

            [maxvertexcount(3)]
            void geom(point v2g patch[1], inout TriangleStream<g2f> triStream)
            {
                v2g triangulo = patch[0];

                float3 normal = triangulo.normal;

                g2f vertice1;
                vertice1.vertex = UnityObjectToClipPos(triangulo.vertexC);
                vertice1.normal = normal;
                vertice1.uv = triangulo.uvC;
                vertice1.uv2 = triangulo.uv2C;
                vertice1.color = triangulo.colorC;
                triStream.Append(vertice1);

                g2f vertice2;
                vertice2.vertex = UnityObjectToClipPos(triangulo.vertexB);
                vertice2.normal = normal;
                vertice2.uv = triangulo.uvB;
                vertice2.uv2 = triangulo.uv2B;
                vertice2.color = triangulo.colorB;
                triStream.Append(vertice2);

                g2f vertice3;
                vertice3.vertex = UnityObjectToClipPos(triangulo.vertexA);
                vertice3.normal = normal;
                vertice3.uv = triangulo.uvA;
                vertice3.uv2 = triangulo.uv2A;
                vertice3.color = triangulo.colorA;
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
