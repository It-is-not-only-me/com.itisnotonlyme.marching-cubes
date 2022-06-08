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
                CrearDatosBuffer(cantidad);
                _cantidadDatosActual = cantidad;
            }
            DatosBuffer.SetCounterValue((uint)(_cantidadDatosActual - cantidad));

            return DatosBuffer;
        }

        private void CrearDatosBuffer(int cantidad)
        {
            if (DatosBuffer != null)
                DatosBuffer.Dispose();
            DatosBuffer = new ComputeBuffer(cantidad, _strideDatos);
        }

        public ComputeBuffer ObtenerTriangulosBuffer(int cantidad)
        {
            if (TriangulosBuffer == null || _cantidadTriangulosActual < cantidad)
            {
                CrearTriangulosBuffer(cantidad);
                _cantidadTriangulosActual = cantidad;
            }
            TriangulosBuffer.SetCounterValue(0);

            return TriangulosBuffer;
        }

        private void CrearTriangulosBuffer(int cantidad)
        {
            if (TriangulosBuffer != null)
                TriangulosBuffer.Dispose();
            TriangulosBuffer = new ComputeBuffer(cantidad, _strideTriangulos);
        }
        
        public void Destruir()
        {
            if (TriangulosBuffer != null) TriangulosBuffer.Dispose();
            if (DatosBuffer != null) DatosBuffer.Dispose();
        }
    }
}
