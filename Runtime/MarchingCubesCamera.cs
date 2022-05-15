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

        [Space]

        [SerializeField] [Range(0, 1)] private float _isoLevel;

        [Space]

        [SerializeField] private DatosEventoSO _obtenerDatosEvento;

        private Material _material;
        private ComputeBuffer _triangulosBuffer, _argBuffer;
        private Dictionary<int, ComputeBuffer> _datosBufferDiccionario;
        private int _triangulos;
        private List<IObtenerDatos> _datos;


        private void Awake()
        {
            _material = new Material(_geometryShader);
            _datos = new List<IObtenerDatos>();
            _datosBufferDiccionario = new Dictionary<int, ComputeBuffer>();
            CrearArgBuffer();
            _triangulos = -1;
        }

        private void OnEnable() => _obtenerDatosEvento.ObtenerDatosEvento += RecibirDatos;
        private void OnDisable() => _obtenerDatosEvento.ObtenerDatosEvento -= RecibirDatos;

        private void OnRenderObject()
        {
            ConfigurarBuffers();

            Render();

            _datos.Clear();
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
            _datos.Add(datos);
        }

        private void ConfigurarBuffers()
        {
            int datosTotales = 0;
            _datos.ForEach(dato => datosTotales += dato.Cantidad);
            if (datosTotales < 8)
                return;

            // 5 es la cantidad de triangulos maximos que va a tener un cube segun el algoritmo
            int triangulosCount = datosTotales * 5;
            int trianguloStride = 3 * (3 * sizeof(float));

            if (_triangulos != triangulosCount)
            {
                if (_triangulosBuffer != null)
                    _triangulosBuffer.Dispose();
                _triangulosBuffer = new ComputeBuffer(triangulosCount, trianguloStride, ComputeBufferType.Append);
                _triangulos = triangulosCount;
            }
            else
            {
                _triangulosBuffer.SetCounterValue(0);
            }

            foreach (IObtenerDatos datos in _datos)
            {
                int datosCount = datos.Cantidad;
                int datosStride = 4 * sizeof(float);
                ComputeBuffer datosBuffer = ObtenerDatosBuffer(datos.Id, datosCount, datosStride);
                datosBuffer.SetCounterValue(0);

                Dato[] datosObtenidos = new Dato[datosCount];
                int contador = 0;
                foreach (Dato dato in datos.GetDatos())
                {
                    datosObtenidos[contador] = dato;
                    contador++;
                }
                datosBuffer.SetData(datosObtenidos);
                Dispatch(datos, datosBuffer);
            }
        }

        ComputeBuffer ObtenerDatosBuffer(int id, int cantidad, int stride)
        {
            ComputeBuffer datos;

            if (!_datosBufferDiccionario.ContainsKey(id))
            {
                datos = new ComputeBuffer(cantidad, stride);
                _datosBufferDiccionario.Add(id, datos);
            }
            else
            {
                datos = _datosBufferDiccionario[id];
            }


            if (datos.count != cantidad)
            {
                datos.Dispose();
                _datosBufferDiccionario[id] = new ComputeBuffer(cantidad, stride);
                datos = _datosBufferDiccionario[id];
            }

            return datos;
        }

        private void Dispatch(IObtenerDatos datos, ComputeBuffer datosBuffer)
        {
            int kernel = _computeShader.FindKernel("March");
            Vector3Int dimension = datos.Dimension;

            _computeShader.SetBuffer(kernel, "triangles", _triangulosBuffer);
            _computeShader.SetBuffer(kernel, "datos", datosBuffer);
            _computeShader.SetFloat("isoLevel", _isoLevel);
            _computeShader.SetInts("numPointsPerAxis", dimension.x, dimension.y, dimension.z);

            _computeShader.Dispatch(kernel, dimension.x, dimension.y, dimension.z);            
        }

        private void Render()
        {
            int[] args = new int[] { 0, 1, 0, 0 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(_triangulosBuffer, _argBuffer, 0);
            _argBuffer.GetData(args);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", _triangulosBuffer);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer);
        }

        private void DestruirBuffers()
        {
            foreach (KeyValuePair<int, ComputeBuffer> par in _datosBufferDiccionario)
                (par.Value).Dispose();

            _triangulosBuffer.Dispose();
            _argBuffer.Dispose();
        }
    }
}
