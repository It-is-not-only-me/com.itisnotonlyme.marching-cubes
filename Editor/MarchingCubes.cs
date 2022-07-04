using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
	public static class MarchingCubes
	{
		static private int _cantidadDeFloatDatos = 6;
		static private int _cantidadDeFloatTriangulos = 3 * 3 + 2 * 3 + 2 * 3 + 3 * 3;
		static private int _triangulosPorDato = 5;

		struct Triangle
		{
			public Vector3 vertexA;
			public Vector3 vertexB;
			public Vector3 vertexC;

			public Vector2 uvA;
			public Vector2 uvB;
			public Vector2 uvC;

			public Vector2 uv2A;
			public Vector2 uv2B;
			public Vector2 uv2C;

			public Vector3 colorA;
			public Vector3 colorB;
			public Vector3 colorC;

			public Vector3 GetVertice(int i)
			{
				switch (i)
				{
					case 0: return vertexA;
					case 1: return vertexB;
					default: return vertexC;
				}
			}

			public Vector2 GetUv(int i)
			{
				switch (i)
				{
					case 0: return uvA;
					case 1: return uvB;
					default: return uvC;
				}
			}

			public Vector3 GetNormales() => (Vector3.Cross(vertexB - vertexA, vertexC - vertexA)).normalized;
		}

		public static Mesh CrearMesh(IObtenerDatos obtenerDatos, IDatoRender datosRender)
		{
			ComputeBuffer datosBuffer, triangulosBuffer;

			Vector3Int puntosPorEje = obtenerDatos.NumeroDePuntosPorEje;
			int datosCount = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;
			MarchingCubeMesh mesh = obtenerDatos.MarchingCubeMesh;
			
			datosBuffer = new ComputeBuffer(datosCount, _cantidadDeFloatDatos * sizeof(float));
			datosBuffer.SetData(mesh.Datos);

			int triangulosCount = datosCount * _triangulosPorDato;
			triangulosBuffer = new ComputeBuffer(triangulosCount, _cantidadDeFloatTriangulos * sizeof(float), ComputeBufferType.Append);

			Dispatch(datosRender, puntosPorEje, datosBuffer, triangulosBuffer);

			Triangle[] triangulos = RecuperarTriangulos(triangulosBuffer);
			Mesh meshResultado = GenerarMesh(triangulos);

			datosBuffer.Dispose();
			triangulosBuffer.Dispose();

			return meshResultado;
		}

		private static Triangle[] RecuperarTriangulos(ComputeBuffer triangulosBuffer)
        {
			ComputeBuffer countBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
			ComputeBuffer.CopyCount(triangulosBuffer, countBuffer, 0);
			int[] triCountArray = { 0 };
			countBuffer.GetData(triCountArray);
			int numTriangulos = triCountArray[0];

			Triangle[] triangulos = new Triangle[numTriangulos];
			triangulosBuffer.GetData(triangulos);

			countBuffer.Dispose();

			return triangulos;
		}

		private static void Dispatch(IDatoRender datosRender, Vector3Int puntosPorEje, ComputeBuffer datosBuffer, ComputeBuffer triangulosBuffer)
		{
			int kernel = datosRender.ComputeShader().FindKernel("March");
			datosRender.ComputeShader().SetBuffer(kernel, "triangles", triangulosBuffer);
			datosRender.ComputeShader().SetBuffer(kernel, "datos", datosBuffer);
			datosRender.ComputeShader().SetFloats("isoLevel", datosRender.IsoLevel());
			datosRender.ComputeShader().SetInts("numPointsPerAxis", puntosPorEje.x, puntosPorEje.y, puntosPorEje.z);

			datosRender.ComputeShader().Dispatch(kernel, puntosPorEje.x - 1, puntosPorEje.y - 1, puntosPorEje.z - 1);
		}

		private static Mesh GenerarMesh(Triangle[] triangulos)
		{
			Mesh meshResultado = new Mesh();
			int subMesh = 0;
			int uvChannel = 1;

			List<Vector3> vertices = new List<Vector3>();
			List<int> indiceTriangulos = new List<int>();
			List<Vector2> uv = new List<Vector2>();
			List<Vector3> normales = new List<Vector3>();

			for (int i = 0; i < triangulos.Length; i++)
				for (int j = 0; j < 3; j++)
				{
					indiceTriangulos.Add(vertices.Count);
					vertices.Add(triangulos[i].GetVertice(j));
					uv.Add(triangulos[i].GetUv(j));
					normales.Add(-triangulos[i].GetNormales());
				}

			meshResultado.SetVertices(vertices);
			meshResultado.SetTriangles(indiceTriangulos, subMesh);
			meshResultado.SetUVs(uvChannel, uv);
			meshResultado.SetNormals(normales);

			return meshResultado;
		}
	}
}