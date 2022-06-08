using System;
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

        private void Awake()
        {
            Material nuevoMaterial = new Material(_datosRender.GeometryShader);
            if (_material != null)
                nuevoMaterial.CopyPropertiesFromMaterial(_material);
            _material = nuevoMaterial;

            CrearArgBuffer();
            _bufferManager = new BufferManager(_datosRender.ComputeShader, _datosRender.IsoLevel);
            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
            RecibirDatos(_generador);
        }

        private void Update()
        {
            if (_generador.Actualizar)
                ActualizarDatos(_generador);
            _bufferManager.ConfigurarBuffer();
            Render();            
        }

        private void OnDestroy() => DestruirBuffers();

        private void CrearArgBuffer()
        {
            int argCount = 4;
            int argStride = sizeof(int);
            _argBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);
        }

        private void RecibirDatos(IObtenerDatos datos)
        {
            _bufferManager.AgregarDatos(datos);
        }

        private void ActualizarDatos(IObtenerDatos datos)
        {
            _bufferManager.ActualizarDatos(datos);
        }

        private void Render()
        {
            ComputeBuffer triangulosBuffer = _bufferManager.Triangulos();
            int[] args = new int[] { 0, 1, 0, 0};
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(triangulosBuffer, _argBuffer, 0);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", triangulosBuffer);

            Bounds bounds = new Bounds(transform.position, _generador.Dimension / 2);
            Graphics.DrawProceduralIndirect(_material, bounds, MeshTopology.Points, _argBuffer);
            //Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer);
        }

        private void DestruirBuffers()
        {
            _argBuffer.Dispose();
            _bufferManager.DestruirBuffer();
        }
    }
}
