using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [AddComponentMenu("Marching cubes/Camara manager")]
    public class MarchingCubesCamera : MonoBehaviour
    {
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private Shader _geometryShader;
        [SerializeField] private Material _material;

        [Space]

        [SerializeField] [Range(0, 1)] private float _isoLevel;

        [Space]

        [SerializeField] private DatosEventoSO _obtenerDatosEvento, _sacarDatosEvento, _actualizarDatosEvento;

        private ComputeBuffer _argBuffer;
        private BufferManager _bufferManager;


        private void Awake()
        {
            if (_material == null)
                _material = new Material(_geometryShader);
            CrearArgBuffer();
            _bufferManager = new BufferManager(_computeShader, _isoLevel);
        }

        private void OnEnable()
        {
            if (_obtenerDatosEvento != null)
                _obtenerDatosEvento.ObtenerDatosEvento += RecibirDatos;
            
            if (_sacarDatosEvento != null)
                _sacarDatosEvento.ObtenerDatosEvento += EliminarDatos;
            
            if (_actualizarDatosEvento != null)
                _actualizarDatosEvento.ObtenerDatosEvento += ActualizarDatos;
        } 

        private void OnDisable()
        {
            if (_obtenerDatosEvento != null)
                _obtenerDatosEvento.ObtenerDatosEvento -= RecibirDatos;

            if (_sacarDatosEvento != null)
                _sacarDatosEvento.ObtenerDatosEvento -= EliminarDatos;

            if (_actualizarDatosEvento != null)
                _actualizarDatosEvento.ObtenerDatosEvento -= ActualizarDatos;
        } 

        private void OnRenderObject()
        {
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

        private void EliminarDatos(IObtenerDatos datos)
        {
            _bufferManager.SacarDatos(datos);
        }

        private void ActualizarDatos(IObtenerDatos datos)
        {
            _bufferManager.ActualizarDatos(datos);
        }

        private void Render()
        {
            ComputeBuffer triangulosBuffer = _bufferManager.Triangulos();
            int[] args = new int[] { 0, 1, 0, 0 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(triangulosBuffer, _argBuffer, 0);
            _argBuffer.GetData(args);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", triangulosBuffer);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer);
        }

        private void DestruirBuffers()
        {
            _argBuffer.Dispose();
            _bufferManager.DestruirBuffer();
        }
    }
}
