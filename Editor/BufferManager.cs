using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class BufferManager
    {
        private List<DatosBuffer> _datos;
        private Dictionary<int, int> _datosDiccionario;

        private ComputeShader _computeShader;
        private float _isoLevel;

        private ComputeBufferPool _poolComputeBuffer;

        private class DatosBuffer
        {
            public IObtenerDatos Dato;
            public TriangulosBufferManager TrianguloManager;

            public DatosBuffer(IObtenerDatos dato, TriangulosBufferManager trianguloManager)
            {
                Dato = dato;
                TrianguloManager = trianguloManager;
            }
        }

        public BufferManager(ComputeShader computeShader, float isoLevel)
        {
            _datos = new List<DatosBuffer>();
            _datosDiccionario = new Dictionary<int, int>();
            _poolComputeBuffer = new ComputeBufferPool();

            _computeShader = computeShader;
            _isoLevel = isoLevel;
        }

        public void AgregarDatos(IObtenerDatos datos)
        {
            int posicion = _datos.Count;
            _datos.Add(new DatosBuffer(datos, new TriangulosBufferManager()));
            _datosDiccionario.Add(datos.Id, posicion);
        }

        public void SacarDatos(IObtenerDatos datos)
        {
            if (!_datosDiccionario.ContainsKey(datos.Id))
                return;

            int posicion = _datosDiccionario[datos.Id];
            DestruirElementoDiccionario(_datos[posicion].TrianguloManager);

            _datos.RemoveAt(posicion);
            for (int i = posicion; i < _datos.Count; i++)
            {
                IObtenerDatos datoActual = _datos[posicion].Dato;
                _datosDiccionario[datoActual.Id] = i;
            }    
        }

        public void ActualizarDatos(IObtenerDatos datos)
        {
            if (!_datosDiccionario.ContainsKey(datos.Id)) 
                return;

            int datosCount = Cantidad(datos);
            int datosStride = 4 * sizeof(float);
            ComputeBuffer datosBuffer = _poolComputeBuffer.ConseguirComputeBuffer(datosCount, datosStride);

            LlenarDatos(datos, datosBuffer);

            int triangulosCount = datosCount * 5;
            int triangulosStride = 3 * (3 * sizeof(float));
            int posicion = _datosDiccionario[datos.Id];
            ComputeBuffer triangulosbuffer = _datos[posicion].TrianguloManager.NuevoTriangulos(triangulosCount, triangulosStride);

            Dispatch(datos, datosBuffer, triangulosbuffer);
        }

        public IEnumerable<ComputeBuffer> Triangulos()
        {
            foreach (DatosBuffer datos in _datos)
                yield return datos.TrianguloManager.Triangulos;
        }

        private void LlenarDatos(IObtenerDatos datos, ComputeBuffer datosBuffer)
        {
            Dato[] datosObtenidos = new Dato[Cantidad(datos)];

            int contador = 0;
            foreach (Dato dato in datos.GetDatos())
            {
                datosObtenidos[contador] = dato;
                contador++;
            }
            datosBuffer.SetData(datosObtenidos);
        }

        private void Dispatch(IObtenerDatos datos, ComputeBuffer datosBuffer, ComputeBuffer triangulosBuffer)
        {
            int kernel = _computeShader.FindKernel("March");
            Vector3Int dimension = datos.Dimension;

            _computeShader.SetBuffer(kernel, "triangles", triangulosBuffer);
            _computeShader.SetBuffer(kernel, "datos", datosBuffer);
            _computeShader.SetFloat("isoLevel", _isoLevel);
            _computeShader.SetInts("numPointsPerAxis", dimension.x, dimension.y, dimension.z);

            _computeShader.Dispatch(kernel, dimension.x, dimension.y, dimension.z);
        }

        private int Cantidad(IObtenerDatos datos)
        {
            Vector3Int dimension = datos.Dimension;
            return dimension.x * dimension.y * dimension.z;
        }

        public void Destruir()
        {
            for (int i = _datos.Count - 1; i >= 0; i--)
                SacarDatos(_datos[i].Dato);
            _poolComputeBuffer.Destruir();
        }

        private void DestruirElementoDiccionario(TriangulosBufferManager triangulosManager)
        {
            triangulosManager.Destruir();
        }
    }
}
