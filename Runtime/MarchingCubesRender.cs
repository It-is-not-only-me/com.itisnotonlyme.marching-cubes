using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [AddComponentMenu("Marching cubes/Camara manager")]
    public class MarchingCubesRender : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private DatosRender _datosRender;

        private ComputeBuffer _argBuffer;
        private BufferManager _bufferManager;
        private GenerarDatos _generador;

        private int _cantidadDeFloatDatos = 6;
        private int _cantidadDeFloatTriangulos = 9;
        private int _triangulosPorDato = 5;

        private void Awake()
        {
            Material nuevoMaterial = new Material(_datosRender.GeometryShader);
            if (_material != null)
                nuevoMaterial.CopyPropertiesFromMaterial(_material);
            _material = nuevoMaterial;
            CrearArgBuffer();

            int datosStride = _cantidadDeFloatDatos * sizeof(float);
            int triangulosStride = _cantidadDeFloatTriangulos * sizeof(float);

            _bufferManager = new BufferManager(datosStride, triangulosStride);
            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
        }

        private void OnDestroy()
        {
            if (_argBuffer != null) _argBuffer.Dispose();
            _bufferManager.Destruir();
        }

        private void Update()
        {
            if (_generador.Actualizar)
                ActualizarDatos();

            ConfigurarBuffer();
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
            int datosCount = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;
            ComputeBuffer datosBuffer = _bufferManager.ObtenerDatosBuffer(datosCount);
            datosBuffer.SetData(_generador.GetDatos());
        }

        private void ConfigurarBuffer()
        {
            Vector3Int puntosPorEje = _generador.NumeroDePuntosPorEje;
            int datosCount = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;
            int triangulosCount = datosCount * _triangulosPorDato;
            ComputeBuffer triangulosBuffer = _bufferManager.ObtenerTriangulosBuffer(triangulosCount);

            int kernel = _datosRender.ComputeShader.FindKernel("March");
            _datosRender.ComputeShader.SetBuffer(kernel, "triangles", triangulosBuffer);
            _datosRender.ComputeShader.SetBuffer(kernel, "datos", _bufferManager.DatosBuffer);
            _datosRender.ComputeShader.SetFloats("isoLevel", _datosRender.IsoLevel);
            _datosRender.ComputeShader.SetInts("numPointsPerAxis", puntosPorEje.x, puntosPorEje.y, puntosPorEje.z);

            _datosRender.ComputeShader.Dispatch(kernel, puntosPorEje.x, puntosPorEje.y, puntosPorEje.z);
        }

        private void Render()
        {
            ComputeBuffer triangulosBuffer = _bufferManager.TriangulosBuffer;
            int[] args = new int[] { 0, 1, 0, 0 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(triangulosBuffer, _argBuffer, 0);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", triangulosBuffer);

            Graphics.DrawProceduralIndirect(_material, _generador.Bounds, MeshTopology.Points, _argBuffer);
        }
    }
}
