using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class BufferManager
    {
        public ComputeBuffer TriangulosBuffer { get; private set; }
        public ComputeBuffer DatosBuffer { get; private set; }
        public ComputeBuffer IndicesBuffer { get; private set; }

        private int _cantidadDatosActual, _cantidadTriangulosActual, _cantidadIndicesActual;
        private int _strideDatos, _strideTriangulos, _strideIndices;

        public BufferManager(int strideDatos, int strideTriangulos, int strideIndices)
        {
            _strideDatos = strideDatos;
            _strideTriangulos = strideTriangulos;
            _strideIndices = strideIndices;
        }

        public ComputeBuffer ObtenerDatosBuffer(int cantidad)
        {
            if (DatosBuffer == null || _cantidadDatosActual < cantidad)
            {
                DatosBuffer = CrearDatosBuffer(cantidad);
                _cantidadDatosActual = cantidad;
            }
            DatosBuffer.SetCounterValue((uint)(_cantidadDatosActual - cantidad));

            return DatosBuffer;
        }

        private ComputeBuffer CrearDatosBuffer(int cantidad)
        {
            DatosBuffer?.Dispose();
            return new ComputeBuffer(cantidad, _strideDatos);
        }

        public ComputeBuffer ObtenerTriangulosBuffer(int cantidad)
        {
            if (TriangulosBuffer == null || _cantidadTriangulosActual < cantidad)
            {
                TriangulosBuffer = CrearTriangulosBuffer(cantidad);
                _cantidadTriangulosActual = cantidad;
            }
            TriangulosBuffer.SetCounterValue(0);

            return TriangulosBuffer;
        }

        private ComputeBuffer CrearTriangulosBuffer(int cantidad)
        {

            TriangulosBuffer?.Dispose();
            return new ComputeBuffer(cantidad, _strideTriangulos, ComputeBufferType.Append);
        }
        
        public ComputeBuffer ObtenerIndicesBuffer(int cantidad)
        {
            if (IndicesBuffer == null || _cantidadIndicesActual < cantidad)
            {
                IndicesBuffer = CrearIndicesBuffer(cantidad);
                _cantidadTriangulosActual = cantidad;
            }
            IndicesBuffer.SetCounterValue(0);

            return IndicesBuffer;
        }

        private ComputeBuffer CrearIndicesBuffer(int cantidad)
        {

            IndicesBuffer?.Dispose();
            return new ComputeBuffer(cantidad, _strideIndices);
        }

        public void Destruir()
        {
            TriangulosBuffer?.Dispose();
            DatosBuffer?.Dispose();
            IndicesBuffer?.Dispose();
        }
    }
}
