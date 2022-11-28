using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
	public static class MarchingCubes
	{
		static private int _cantidadDeFloatDatos = 4;
		static private int _cantidadDeFloatTriangulos = 3 * 3 + 2 * 3 + 2 * 3 + 3 * 3;
		static private int _triangulosPorDato = 5;
		static private int _cantidadDeFloatUvs = 4;

		struct Vertice
        {
			public Vector3 vertex;
			public Vector2 uv;
			public Vector2 uv2;
			public Vector3 color;
        }

		struct Triangle
		{
			public Vertice verticeA;
			public Vertice verticeB;
			public Vertice verticeC;

			public Vector3 GetVertice(int i)
			{
				switch (i)
				{
					case 0: return verticeA.vertex;
					case 1: return verticeB.vertex;
					default: return verticeC.vertex;
				}
			}

			public Vector4 GetUv(int i)
			{
				switch (i)
				{
					case 0: return verticeA.uv;
					case 1: return verticeB.uv;
					default: return verticeC.uv;
				}
			}

			public Vector3 GetNormales() => (Vector3.Cross(GetVertice(1) - GetVertice(0), GetVertice(2) - GetVertice(0))).normalized;
		}

		static private ComputeBuffer _datosBuffer, _indicesBuffer, _triangulosBuffer, _uvsBuffers;

		public static Mesh CrearMesh(IObtenerDatos obtenerDatos, IDatoRender datosRender)
		{
			MarchingCubeMesh mesh = obtenerDatos.MarchingCubeMesh;
			int cantidadDeDatos = mesh.Datos.Length;
			
			_datosBuffer = new ComputeBuffer(cantidadDeDatos, _cantidadDeFloatDatos * sizeof(float));
			_datosBuffer.SetData(mesh.Datos);

			int cantidadDeTriangulos = cantidadDeDatos * _triangulosPorDato;
			_triangulosBuffer = new ComputeBuffer(cantidadDeTriangulos, _cantidadDeFloatTriangulos * sizeof(float), ComputeBufferType.Append);

			int cantidadDeIndices = mesh.Indices.Length;
			_indicesBuffer = new ComputeBuffer(cantidadDeIndices, sizeof(int));
			_indicesBuffer.SetData(mesh.Indices);

			int cantidadDeUvs = cantidadDeDatos;
			_uvsBuffers = new ComputeBuffer(cantidadDeUvs, _cantidadDeFloatUvs * sizeof(float));
			_uvsBuffers.SetData(mesh.Uvs);

			Dispatch(datosRender);

			Triangle[] triangulos = RecuperarTriangulos(_triangulosBuffer);
			Mesh meshResultado = GenerarMesh(triangulos);

			_datosBuffer.Dispose();
			_triangulosBuffer.Dispose();
			_indicesBuffer.Dispose();
			_uvsBuffers.Dispose();

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

		private static void Dispatch(IDatoRender datosRender)
		{
			int kernel = datosRender.ComputeShader().FindKernel("March");

			int cantidadIndices = _indicesBuffer.count;
			int cantidadPorEjes = Mathf.CeilToInt(Mathf.Pow(cantidadIndices / 8, 1.0f / 3.0f));

			datosRender.ComputeShader().SetBuffer(kernel, "triangles", _triangulosBuffer);
			datosRender.ComputeShader().SetBuffer(kernel, "datos", _datosBuffer);
			datosRender.ComputeShader().SetBuffer(kernel, "indices", _indicesBuffer);
			datosRender.ComputeShader().SetBuffer(kernel, "uvs", _uvsBuffers);

			datosRender.ComputeShader().SetInt("cantidadPorEje", cantidadPorEjes);
			datosRender.ComputeShader().SetInt("cantidadIndices", cantidadIndices);
			datosRender.ComputeShader().SetFloats("isoLevel", datosRender.IsoLevel());

			datosRender.ComputeShader().Dispatch(kernel, cantidadPorEjes, cantidadPorEjes, cantidadPorEjes);
		}

		private static Mesh GenerarMesh(Triangle[] triangulos)
		{
			Mesh meshResultado = new Mesh();
			int subMesh = 0;
			int uvChannel = 1;

			List<Vector3> vertices = new List<Vector3>();
			List<int> indiceTriangulos = new List<int>();
			List<Vector2> uvs = new List<Vector2>();
			List<Vector2> uvs2 = new List<Vector2>();
			List<Vector3> normales = new List<Vector3>();

			for (int i = 0; i < triangulos.Length; i++)
				for (int j = 0; j < 3; j++)
				{
					indiceTriangulos.Add(vertices.Count);
					vertices.Add(triangulos[i].GetVertice(j));
					Vector4 uvsActual = triangulos[i].GetUv(j);
                    uvs.Add(new Vector2(uvsActual.x, uvsActual.y));
					uvs2.Add(new Vector2(uvsActual.z, uvsActual.w));
					normales.Add(-triangulos[i].GetNormales());
				}

			meshResultado.SetVertices(vertices);
			meshResultado.SetTriangles(indiceTriangulos, subMesh);
			meshResultado.SetUVs(uvChannel, uvs);
			meshResultado.SetUVs(uvChannel + 1, uvs2);
			meshResultado.SetNormals(normales);

			return meshResultado;
		}
	}
}