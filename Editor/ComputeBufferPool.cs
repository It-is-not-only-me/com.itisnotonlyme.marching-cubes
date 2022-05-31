using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class ComputeBufferPool
    {
        private ComputeBuffer _computeBuffer;
        private int _tamanioActual = 0;
        private int _strideActual = 0;

        public ComputeBuffer ConseguirComputeBuffer(int tamanio, int stride)
        {
            if (NecesitaCrearNuevo(tamanio, stride))
                CrearComputeBuffer(tamanio, stride);

            _computeBuffer.SetCounterValue((uint)(_tamanioActual - tamanio));

            return _computeBuffer;
        }

        private bool NecesitaCrearNuevo(int tamanio, int stride)
        {
            if (stride != _strideActual)
                return true;
            return tamanio > _tamanioActual;
        }

        private void CrearComputeBuffer(int tamanio, int stride)
        {
            Destruir();
            _computeBuffer = new ComputeBuffer(tamanio, stride);

            _tamanioActual = tamanio;
            _strideActual = stride;
        }

        public void Destruir()
        {
            if (_computeBuffer != null)
                _computeBuffer.Dispose();
        }
    }
}
