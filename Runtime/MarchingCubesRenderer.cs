using System.Collections;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class MarchingCubesRenderer : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private Camera _camara;
        [SerializeField] private Shader _geometryShader;
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField][Range(0, 1)] private float _isoLevel;

        private GenerarDatos _generador;
        private ComputeBuffer _argBuffer, _triangulosBuffer, _datosBuffer;

        private int _countActual = -1;
        private int _strideTriangulos = 9 * sizeof(float);
        private int _cantidadTriangulosPorDato = 5;
        private int _strideDatos = 6 * sizeof(float);

        private void Awake()
        {
            if (_camara == null)
                _camara = Camera.main;
            if (_material == null)
                _material = new Material(_geometryShader);

            if (!TryGetComponent<GenerarDatos>(out _generador))
                Debug.LogError("No tenes un generador en este gameObject");

            CrearArgBuffer();
        }

        private void Update()
        {
            if (_generador.Actualizar)
                RegenerarMesh();
        }

        private void OnRenderObject()
        {
            int[] args = new int[] { 0, 1, 0, 1 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(_triangulosBuffer, _argBuffer, 0);
            _argBuffer.GetData(args);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", _triangulosBuffer);
            //Bounds bounds = new Bounds(_generador.Posicion, _generador.Tamanio);
            //Graphics.DrawProceduralIndirect(_material, bounds, MeshTopology.Points, _argBuffer, 0, _camara);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer);
        }

        private void RegenerarMesh()
        {
            int datosCount = Cantidad();

            GenerarDatosBuffer(datosCount);
            GenerarTriangulosBuffer(datosCount * _cantidadTriangulosPorDato);

            Dato[] datosObtenidos = new Dato[datosCount];
            int contador = 0;
            foreach (Dato dato in _generador.GetDatos())
            {
                datosObtenidos[contador] = dato;
                contador++;
            }
            _datosBuffer.SetData(datosObtenidos);
            Dispatch();
        }

        private void Dispatch()
        {
            int kernel = _computeShader.FindKernel("March");
            Vector3Int dimension = _generador.Tamanio;
            _computeShader.SetBuffer(kernel, "triangles", _triangulosBuffer);
            _computeShader.SetBuffer(kernel, "datos", _datosBuffer);
            _computeShader.SetFloat("isoLevel", _isoLevel);
            _computeShader.SetInts("numPointsPerAxis", dimension.x, dimension.y, dimension.z);

            _computeShader.Dispatch(kernel, dimension.x, dimension.y, dimension.z);
        }

        private void GenerarDatosBuffer(int count)
        {
            if (_datosBuffer == null || _countActual < count)
            {
                if (_datosBuffer != null)
                    _datosBuffer.Dispose();

                _datosBuffer = new ComputeBuffer(count, _strideDatos);
                _countActual = count;
            }

            _datosBuffer.SetCounterValue(0);
        }

        private void GenerarTriangulosBuffer(int count)
        {
            if (_triangulosBuffer == null || _countActual * _cantidadTriangulosPorDato < count)
            {
                if (_triangulosBuffer != null)
                    _triangulosBuffer.Dispose();
                _triangulosBuffer = new ComputeBuffer(count, _strideTriangulos);
            }

            _triangulosBuffer.SetCounterValue(0);
        }

        private int Cantidad()
        {
            Vector3 dimension = _generador.Tamanio;
            return Mathf.RoundToInt(dimension.x * dimension.y * dimension.z);
        }

        private void CrearArgBuffer()
        {
            int argCount = 4;
            int argStride = sizeof(int);
            _argBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);   
        }

        private void OnDestroy()
        {
            if (_argBuffer != null) _argBuffer.Dispose();

            if (_triangulosBuffer != null) _triangulosBuffer.Dispose();

            if (_datosBuffer != null) _datosBuffer.Dispose();
        }
    }
}
;