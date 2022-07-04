using System;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class BufferManager
    {
        private static uint _startCount = 0;

        public ComputeBuffer Buffer
        {
            get => _buffer;
            set => _buffer = value;
        }

        private ComputeBuffer _buffer;
        private int _stride, _cantidadActual;
        private Func<int, int, ComputeBuffer> _creacionBuffer;

        public BufferManager(int stride, Func<int, int, ComputeBuffer> crearBuffer)
        {
            _stride = stride;
            _creacionBuffer = crearBuffer;
        }

        public ComputeBuffer ObtenerBuffer(int cantidad)
        {
            if (Buffer == null || _cantidadActual < cantidad)
            {
                Buffer = CrearBuffer(cantidad);
                _cantidadActual = cantidad;
            }
            Buffer.SetCounterValue(_startCount);
            return Buffer;
        }

        private ComputeBuffer CrearBuffer(int cantidad)
        {
            Destruir();
            return _creacionBuffer(cantidad, _stride);
        }

        public void Destruir()
        {
            _buffer?.Dispose();
        }
    }
}
