using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class BufferManager
    {
        public ComputeBuffer TriangulosBuffer { get; private set; }
        public ComputeBuffer DatosBuffer { get; private set; }

        private int _cantidadDatosActual, _cantidadTriangulosActual;
        private int _strideDatos, _strideTriangulos;

        public BufferManager(int strideDatos, int strideTriangulos)
        {
            _strideDatos = strideDatos;
            _strideTriangulos = strideTriangulos;
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
        
        public void Destruir()
        {
            TriangulosBuffer?.Dispose();
            DatosBuffer?.Dispose();
        }
    }
}
