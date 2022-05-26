using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{    
    public class BufferManager
    {
        private List<IObtenerDatos> _datos;
        private Dictionary<int, InfoBuffer> _datosDiccionario;

        private ComputeBuffer _triangulosBuffer;
        private int _cantidadTriangulos;

        private ComputeShader _computeShader;
        private float _isoLevel;

        private struct InfoBuffer
        {
            public ComputeBuffer DatosBuffer;
            public int Index;
            public bool Actualizado;

            public InfoBuffer(ComputeBuffer datosBuffer, int index = -1, bool actualizado = true)
            {
                DatosBuffer = datosBuffer;
                Index = index;
                Actualizado = actualizado;
            }
        }

        public BufferManager(ComputeShader computeSahder, float isoLevel)
        {
            _datos = new List<IObtenerDatos>();
            _datosDiccionario = new Dictionary<int, InfoBuffer>();

            _computeShader = computeSahder;
            _isoLevel = isoLevel;
        }

        public void AgregarDatos(IObtenerDatos datos)
        {
            _datos.Add(datos);
        }

        public void ActualizarDatos(IObtenerDatos datos)
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
            // poner en actualizar estos datos
        }

        public void SacarDatos(IObtenerDatos datos)
        {
            _datos.Remove(datos);
        }

        // configurar esto para hacer que tenga todo ordenado bien y optimizar todo
        public void ConfigurarBuffer()
        {
            int datosTotales = 0;
            _datos.ForEach(dato => datosTotales += dato.Cantidad);
            if (datosTotales < 8)
                return;

            // 5 es la cantidad de triangulos maximos que va a tener un cube segun el algoritmo
            int triangulosCount = datosTotales * 5;
            int triangulosStride = 3 * (3 * sizeof(float));

            int inicio = 0, index = 0;
            Orden(out inicio, out index);
            
            _triangulosBuffer = TriangulosBuffer(index, ref inicio, triangulosCount, triangulosStride);
            
            // ordenar
            _datos.Sort(inicio, _datos.Count, (x, y) => 
                        {
                            if (!_datosDiccionario[x.Id].Actualizado)
                                return -1;
                            if (!_datosDiccionario[y.Id].Actualizado)
                                return 1;
                        });
            
            for (int i = inicio; i < _datos.Count; i++) 
            {
                
            }

            
                        
            foreach (IObtenerDatos datos in _datos)
            {
                int datosCount = datos.Cantidad;
                int datosStride = 4 * sizeof(float);
                ComputeBuffer datosBuffer = ObtenerDatosBuffer(datos.Id, datosCount, datosStride);
                Dispatch(datos, datosBuffer);
            }
        }
        
        private ComputeBuffer TriangulosBuffer(int indexInicio, ref int orden, int cantidad, int stride) 
        {
            if (cantidad < _cantidadTriangulos)
            {
                // ver si tengo que copiarlo y guardarlo en el nuevo buffer
                if (_triangulosBuffer != null)
                    _triangulosBuffer.Dispose();
                _triangulosBuffer = new ComputeBuffer(cantidad, stride, ComputeBufferType.Append);
                _cantidadTriangulos = cantidad;
                orden = 0;
            }
            else
            {
                _triangulosBuffer.SetCounterValue((uint)indexInicio);
            }
            
            return _triangulosBuffer;
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

        public ComputeBuffer Triangulos()
        {
            return _triangulosBuffer;
        }

        public void DestruirBuffer()
        {
            foreach (KeyValuePair<int, InfoBuffer> par in _datosDiccionario)
                (par.Value.DatosBuffer).Dispose();
            _triangulosBuffer.Dispose();
        }

        private void Orden(out int inicio, out int index)
        {
            inicio = 0;
            index = 0;
            
            for (IObtenerDatos datos in _datos)
            {
                InfoBuffer infoBuffer = _datosDiccionario[datos.Id];
                if (!infoBuffer.Actualizado)
                {
                    inicio++;
                    index = infoBuffer.Index;
                }
                else
                {
                    break;
                }
            }                
        }

        private ComputeBuffer ObtenerDatosBuffer(int id, int cantidad, int stride)
        {
            ComputeBuffer datos;

            if (!_datosDiccionario.ContainsKey(id))
            {
                datos = new ComputeBuffer(cantidad, stride);
                InfoBuffer nuevoInfoBuffer = new InfoBuffer(datos);
                _datosDiccionario.Add(id, nuevoInfoBuffer);
            }
            else
            {
                datos = _datosDiccionario[id].DatosBuffer;
            }


            if (datos.count != cantidad)
            {
                datos.Dispose();
                InfoBuffer nuevoInfoBuffer = _datosDiccionario[id];
                nuevoInfoBuffer.DatosBuffer = new ComputeBuffer(cantidad, stride);
                _datosDiccionario[id] = nuevoInfoBuffer;
                datos = nuevoInfoBuffer.DatosBuffer;
            }

            return datos;
        }
    }
}
