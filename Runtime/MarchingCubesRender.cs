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
        private BufferManager _bufferDatos, _bufferTriangulos, _bufferIndices, _bufferUvs;
        private GenerarDatos _generador;

        private int _cantidadDeFloatDatos = 4;
        private int _cantidadDeFloatTriangulos = 3 * 3;
        private int _triangulosPorDato = 5;
        private int _cantidadDeFloatUvs = 2;

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

            _bufferDatos = new BufferManager(datosStride, (cantidad, stride) => new ComputeBuffer(cantidad, stride));
            _bufferTriangulos = new BufferManager(triangulosStride, (cantidad, stride) => new ComputeBuffer(cantidad, stride, ComputeBufferType.Append));
            _bufferIndices = new BufferManager(indicesStride, (cantidad, stride) => new ComputeBuffer(cantidad, stride));
            _bufferUvs = new BufferManager(uvsStride, (cantidad, stride) => new ComputeBuffer(cantidad, stride));

            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
        }

        private void OnDestroy()
        {
            _argBuffer?.Dispose();
            _bufferDatos.Destruir();
            _bufferTriangulos.Destruir();
            _bufferIndices.Destruir();
            _bufferUvs.Destruir();
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

            ComputeBuffer datosBuffer = _bufferDatos.ObtenerBuffer(cantidadDeDatos);
            datosBuffer.SetData(mesh.Datos);

            int triangulosCount = cantidadDeDatos * _triangulosPorDato;
            ComputeBuffer triangulosBuffer = _bufferTriangulos.ObtenerBuffer(triangulosCount);

            int cantidadDeindices = ((puntosPorEje.x - 1) * (puntosPorEje.y - 1) * (puntosPorEje.z - 1)) * 8;
            ComputeBuffer indicesBuffer = _bufferIndices.ObtenerBuffer(cantidadDeindices);
            indicesBuffer.SetData(mesh.Indices);

            int cantidadDeUvs = cantidadDeDatos;
            ComputeBuffer uvsBuffer = _bufferUvs.ObtenerBuffer(cantidadDeUvs);

            Dispatch(datosBuffer, indicesBuffer, triangulosBuffer, uvsBuffer);
        }

        private void Dispatch(ComputeBuffer datosBuffer, ComputeBuffer indicesBuffer, ComputeBuffer triangulosBuffer, ComputeBuffer uvsBuffer)
        {
            int kernel = _datosRender.ComputeShader().FindKernel("March");

            int cantidadIndices = indicesBuffer.count;
            int cantidadPorEjes = Mathf.CeilToInt(Mathf.Pow(cantidadIndices / 8, 1.0f / 3.0f));

            _datosRender.ComputeShader().SetBuffer(kernel, "triangles", triangulosBuffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "datos", datosBuffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "indices", indicesBuffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "uvs", uvsBuffer);

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
