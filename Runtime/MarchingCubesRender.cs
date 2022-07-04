using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [AddComponentMenu("Marching cubes/Marching cubes renderer")]
    public class MarchingCubesRender : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private DatosRender _datosRender;

        private ComputeBuffer _argBuffer;
        private BufferManager _bufferDatos, _bufferTriangulos, _bufferIndices, _bufferUvs, _bufferUvs2, _bufferColores;
        private GenerarDatos _generador;

        private int _cantidadDeFloatDatos = 4;
        private int _cantidadDeFloatTriangulos = 3 * 3;
        private int _triangulosPorDato = 5;
        private int _cantidadDeFloatUvs = 2;
        private int _cantidadDeFloatColores = 4;

        private void Awake()
        {
            Material nuevoMaterial = new Material(_datosRender.GeometryShader());
            nuevoMaterial?.CopyPropertiesFromMaterial(_material);
            _material = nuevoMaterial;
            CrearArgBuffer();

            int datosStride = _cantidadDeFloatDatos * sizeof(float);
            int triangulosStride = _cantidadDeFloatTriangulos * sizeof(float);
            int indicesStride = sizeof(int);
            int uvsStride = _cantidadDeFloatUvs * sizeof(float);
            int coloresStride = _cantidadDeFloatColores * sizeof(float);

            _bufferDatos = new BufferManager(datosStride, CrearBuffer);
            _bufferTriangulos = new BufferManager(triangulosStride, CrearBufferAppend);
            _bufferIndices = new BufferManager(indicesStride, CrearBuffer);
            _bufferUvs = new BufferManager(uvsStride, CrearBuffer);
            _bufferUvs2 = new BufferManager(uvsStride, CrearBuffer);
            _bufferColores = new BufferManager(coloresStride, CrearBuffer);

            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
        }

        private ComputeBuffer CrearBuffer(int cantidad, int stride) => new ComputeBuffer(cantidad, stride);
        private ComputeBuffer CrearBufferAppend(int cantidad, int stride) => new ComputeBuffer(cantidad, stride, ComputeBufferType.Append);

        private void OnDestroy()
        {
            _argBuffer?.Dispose();

            _bufferDatos.Destruir();
            _bufferTriangulos.Destruir();
            _bufferIndices.Destruir();
            _bufferUvs.Destruir();
            _bufferUvs2.Destruir();
            _bufferColores.Destruir();
        }

        private void Update()
        {
            if (_generador.Actualizar)
                ActualizarDatos();
            Render();
        }

        public void CrearArgBuffer()
        {
            int argCount = 4;
            int argStride = sizeof(int);
            _argBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);
        }

        private void ActualizarDatos()
        {
            Vector3Int puntosPorEje = _generador.NumeroDePuntosPorEje;
            int cantidadDeDatos = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;
            MarchingCubeMesh mesh = _generador.MarchingCubeMesh;

            _bufferDatos.ObtenerBuffer(cantidadDeDatos).SetData(mesh.Datos);

            int triangulosCount = cantidadDeDatos * _triangulosPorDato;
            _bufferTriangulos.ObtenerBuffer(triangulosCount);

            int cantidadDeindices = ((puntosPorEje.x - 1) * (puntosPorEje.y - 1) * (puntosPorEje.z - 1)) * 8;
            _bufferIndices.ObtenerBuffer(cantidadDeindices).SetData(mesh.Indices);

            int cantidadDeUvs = cantidadDeDatos;
            _bufferUvs.ObtenerBuffer(cantidadDeUvs).SetData(mesh.Uvs);

            int cantidadDeUvs2 = cantidadDeDatos;
            _bufferUvs2.ObtenerBuffer(cantidadDeUvs2).SetData(mesh.Uvs2);

            int cantidadDeColores = cantidadDeDatos;
            _bufferColores.ObtenerBuffer(cantidadDeColores).SetData(mesh.Colores);

            Dispatch();
        }

        private void Dispatch()
        {
            int kernel = _datosRender.ComputeShader().FindKernel("March");

            int cantidadIndices = _bufferDatos.Buffer.count;
            int cantidadPorEjes = Mathf.CeilToInt(Mathf.Pow(cantidadIndices / 8, 1.0f / 3.0f));

            _datosRender.ComputeShader().SetBuffer(kernel, "triangles", _bufferTriangulos.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "datos", _bufferDatos.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "indices", _bufferIndices.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "uvs", _bufferUvs.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "uvs2", _bufferUvs2.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "colores", _bufferColores.Buffer);

            _datosRender.ComputeShader().SetInt("cantidadPorEje", cantidadPorEjes);
            _datosRender.ComputeShader().SetInt("cantidadIndices", cantidadIndices);
            _datosRender.ComputeShader().SetFloats("isoLevel", _datosRender.IsoLevel());            

            _datosRender.ComputeShader().Dispatch(kernel, cantidadPorEjes, cantidadPorEjes, cantidadPorEjes);
        }

        private void Render()
        {
            ComputeBuffer triangulosBuffer = _bufferTriangulos.Buffer;
            int[] args = new int[] { 0, 1, 0, 0 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(triangulosBuffer, _argBuffer, 0);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", triangulosBuffer);

            Graphics.DrawProceduralIndirect(_material, _generador.MarchingCubeMesh.Limites, MeshTopology.Points, _argBuffer);
        }
    }
}
