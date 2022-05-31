using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class TriangulosBufferManager
    {
        private ComputeBuffer _triangulos;
        private int _cantidadActual = -1, _strideActual = -1;

        public ComputeBuffer Triangulos => _triangulos;

        public ComputeBuffer NuevoTriangulos(int cantidad, int stride)
        {
            if (NecesitoCrearNuevo(cantidad, stride))
                CrearNuevo(cantidad, stride);

            _triangulos.SetCounterValue(0);
            return _triangulos;
        }

        private bool NecesitoCrearNuevo(int cantidad, int stride)
        {
            if (stride != _strideActual)
                return true;
            return cantidad > _cantidadActual;
        }

        private void CrearNuevo(int cantidad, int stride)
        {
            Destruir();
            _triangulos = new ComputeBuffer(cantidad, stride, ComputeBufferType.Append);

            _cantidadActual = cantidad;
            _strideActual = stride;
        }

        public void Destruir()
        {
            if (_triangulos != null)
                _triangulos.Dispose();
        }
    }
}