using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{    
    public class BufferManager
    {
        private List<IObtenerDatos> _datos;
        private Dictionary<int, InfoBuffer> _datosDiccionario;

        private ComputeBuffer _triangulosBuffer, _argsBuffer;
        private int _cantidadTriangulos;

        private ComputeShader _computeShader;
        private float _isoLevel;
        private struct InfoBuffer
        {
            public ComputeBuffer DatosBuffer;
            public int Index;
            public bool NecesitaActualizar;

            public InfoBuffer(ComputeBuffer datosBuffer, int index = -1, bool actualizado = true)
            {
                DatosBuffer = datosBuffer;
                Index = index;
                NecesitaActualizar = actualizado;
            }
        }

        public BufferManager(ComputeShader computeSahder, float isoLevel)
        {
            _datos = new List<IObtenerDatos>();
            _datosDiccionario = new Dictionary<int, InfoBuffer>();

            _computeShader = computeSahder;
            _isoLevel = isoLevel;

            int argCount = 4;
            int argStride = sizeof(int);
            _argsBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);

            _cantidadTriangulos = -1;
        }

        public void AgregarDatos(IObtenerDatos datos)
        {
            _datos.Add(datos);
        }

        public void ActualizarDatos(IObtenerDatos datos)
        {
            int datosCount = Cantidad(datos);
            int datosStride = 4 * sizeof(float);
            InfoBuffer infoBuffer = ObtenerInfoBuffer(datos.Id, datosCount, datosStride);
            infoBuffer.DatosBuffer.SetCounterValue(0);
            Dato[] datosObtenidos = new Dato[datosCount];
            int contador = 0;
            foreach (Dato dato in datos.GetDatos())
            {
                datosObtenidos[contador] = dato;
                contador++;
            }

            infoBuffer.DatosBuffer.SetData(datosObtenidos);
            infoBuffer.NecesitaActualizar = true;
            _datosDiccionario[datos.Id] = infoBuffer;
        }

        public void SacarDatos(IObtenerDatos datos)
        {
            _datos.Remove(datos);
            int id = datos.Id;
            if (_datosDiccionario.ContainsKey(id))
            {
                DestruirElementoDiccionario(_datosDiccionario[id]);
                _datosDiccionario.Remove(id);
            }
        }

        public void ConfigurarBuffer()
        {
            int datosTotales = 0;
            _datos.ForEach(dato => datosTotales += Cantidad(dato));
            if (datosTotales < 8)
                return;

            // 5 es la cantidad de triangulos maximos que va a tener un cube segun el algoritmo
            int triangulosCount = datosTotales * 5;
            int triangulosStride = 3 * (3 * sizeof(float));

            int inicio = 0, index = 0;
            Orden(out inicio, out index);
            
            _triangulosBuffer = TriangulosBuffer(index, ref inicio, triangulosCount, triangulosStride);

            OrdenarInformacion(_datos, inicio);

            for (int i = inicio; i < _datos.Count; i++) 
            {
                IObtenerDatos datos = _datos[i];
                int datosCount = Cantidad(datos);
                int datosStride = 4 * sizeof(float);
                InfoBuffer infoBuffer = ObtenerInfoBuffer(datos.Id, datosCount, datosStride);
                infoBuffer.Index = Dispatch(datos, infoBuffer.DatosBuffer);
                infoBuffer.NecesitaActualizar = false;
                _datosDiccionario[datos.Id] = infoBuffer;
            }
        }
        
        private ComputeBuffer TriangulosBuffer(int indexInicio, ref int orden, int cantidad, int stride) 
        {
            if (_cantidadTriangulos < cantidad)
            {
                // ver si tengo que copiarlo y guardarlo en el nuevo buffer
                if (_triangulosBuffer != null)
                    _triangulosBuffer.Dispose();
                cantidad *= 2;
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

        private void OrdenarInformacion(List<IObtenerDatos> datos, int inicio)
        {
            for (int i = inicio; i < datos.Count; i++)
            {
                for (int j = inicio; j < datos.Count - 1; j++)
                {
                    IObtenerDatos dato = datos[j];

                    if (_datosDiccionario[dato.Id].NecesitaActualizar)
                    {
                        IObtenerDatos aux = datos[j + 1];
                        datos[j + 1] = dato;
                        datos[j] = aux;
                    }
                }
            }
        }

        private int Dispatch(IObtenerDatos datos, ComputeBuffer datosBuffer)
        {
            int kernel = _computeShader.FindKernel("March");
            Vector3Int dimension = datos.Dimension;
            _computeShader.SetBuffer(kernel, "triangles", _triangulosBuffer);
            _computeShader.SetBuffer(kernel, "datos", datosBuffer);
            _computeShader.SetFloat("isoLevel", _isoLevel);
            _computeShader.SetInts("numPointsPerAxis", dimension.x, dimension.y, dimension.z);

            _computeShader.Dispatch(kernel, dimension.x, dimension.y, dimension.z);

            int[] args = new int[] { 0, 1, 0, 0 };
            _argsBuffer.SetData(args);
            ComputeBuffer.CopyCount(_triangulosBuffer, _argsBuffer, 0);
            _argsBuffer.GetData(args);

            return args[0];
        }

        public ComputeBuffer Triangulos()
        {
            return _triangulosBuffer;
        }

        private void Orden(out int inicio, out int index)
        {
            inicio = 0;
            index = 0;
            
            foreach (IObtenerDatos datos in _datos)
            {
                InfoBuffer infoBuffer = _datosDiccionario[datos.Id];
                if (!infoBuffer.NecesitaActualizar)
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

        private InfoBuffer ObtenerInfoBuffer(int id, int cantidad, int stride)
        {
            InfoBuffer infoBuffer;

            if (!_datosDiccionario.ContainsKey(id))
            {
                infoBuffer = new InfoBuffer(new ComputeBuffer(cantidad, stride));
                _datosDiccionario.Add(id, infoBuffer);
            }
            else
            {
                infoBuffer = _datosDiccionario[id];
            }

            ComputeBuffer buffer = infoBuffer.DatosBuffer;
            if (buffer.count != cantidad)
            {
                buffer.Dispose();
                infoBuffer.DatosBuffer = new ComputeBuffer(cantidad, stride);
                _datosDiccionario[id] = infoBuffer;
            }

            return infoBuffer;
        }

        public void DestruirBuffer()
        {
            foreach (KeyValuePair<int, InfoBuffer> par in _datosDiccionario)
                DestruirElementoDiccionario(par.Value);
            if (_triangulosBuffer != null)
                _triangulosBuffer.Dispose();
            _argsBuffer.Dispose();
        }

        private void DestruirElementoDiccionario(InfoBuffer infoBuffer)
        {
            infoBuffer.DatosBuffer.Dispose();
        }

        private int Cantidad(IObtenerDatos datos)
        {
            Vector3Int dimension = datos.Dimension;
            return dimension.x * dimension.y * dimension.z;
        }
    }
}
