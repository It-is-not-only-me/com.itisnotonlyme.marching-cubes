using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [AddComponentMenu("Rendering/Marching Cubes Renderer")]
    public class MarchingCubesRender : MonoBehaviour
    {
        [SerializeField] private Material _material;
        [SerializeField] private DatosRender _datosRender;

        private ComputeBuffer _argBuffer, _planosBuffer;
        private Texture2D _datosTexture, _uvsTexture;

        private BufferManager _bufferTriangulos, _bufferIndices;
        private GenerarDatos _generador;
        private Camera _camara;

        private int _cantidadDeFloatTriangulos = 3 * 3 + 2 * 3 + 2 * 3 + 3 * 3;
        private int _triangulosPorDato = 5;
        private int _tamanioTextura;

        private void Awake()
        {
            Material nuevoMaterial = new Material(_datosRender.GeometryShader());
            nuevoMaterial?.CopyPropertiesFromMaterial(_material);
            _material = nuevoMaterial;
            CrearBufferAuxileares();

            _camara = _datosRender.Camara();
            if (_camara == null)
                _camara = Camera.main;

            int triangulosStride = _cantidadDeFloatTriangulos * sizeof(float);
            _bufferTriangulos = new BufferManager(triangulosStride, CrearBufferAppend);

            int indicesStride = sizeof(int);
            _bufferIndices = new BufferManager(indicesStride, CrearBuffer);

            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
        }

        private ComputeBuffer CrearBuffer(int cantidad, int stride) => new ComputeBuffer(cantidad, stride);
        private ComputeBuffer CrearBufferAppend(int cantidad, int stride) => new ComputeBuffer(cantidad, stride, ComputeBufferType.Append);

        private void OnDestroy()
        {
            _argBuffer?.Dispose();
            _planosBuffer?.Dispose();
            
            _bufferTriangulos.Destruir();
            _bufferIndices.Destruir();
        }

        private void FixedUpdate()
        {
            if (_generador.Actualizar)
                Actualizar();
        }

        private void Update() => Render();

        private void Actualizar()
        {
            ActualizarDatos();
            PasarInformacion();
        }

        public void CrearBufferAuxileares()
        {
            int argCount = 4;
            int argStride = sizeof(int);
            _argBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);

            int planosCount = 6;
            int planosStride = 3 * sizeof(float) + sizeof(float);
            _planosBuffer = new ComputeBuffer(planosCount, planosStride);
        }

        private void ActualizarDatos()
        {
            MarchingCubeMesh mesh = _generador.MarchingCubeMesh;

            _datosTexture = mesh.Datos;
            int triangulosCount = mesh.CantidadElementos * _triangulosPorDato;
            _bufferTriangulos.ObtenerBuffer(triangulosCount);

            int cantidadDeindices = mesh.Indices.Length;
            _bufferIndices.ObtenerBuffer(cantidadDeindices).SetData(mesh.Indices);

            _uvsTexture = mesh.Uvs;
            _tamanioTextura = mesh.Dimensiones;

            Dispatch();
        }

        private void Dispatch()
        {
            int kernel = _datosRender.ComputeShader().FindKernel("March");

            int cantidadIndices = _bufferIndices.Buffer.count;
            int cantidadPorEjes = Mathf.CeilToInt(Mathf.Pow(cantidadIndices / 8, 1.0f / 3.0f));

            _datosRender.ComputeShader().SetBuffer(kernel, "triangles", _bufferTriangulos.Buffer);
            _datosRender.ComputeShader().SetTexture(kernel, "datos", _datosTexture);
            _datosRender.ComputeShader().SetBuffer(kernel, "indices", _bufferIndices.Buffer);
            _datosRender.ComputeShader().SetTexture(kernel, "uvs", _uvsTexture);

            _datosRender.ComputeShader().SetInt("anchoTextura", _tamanioTextura);
            _datosRender.ComputeShader().SetInt("cantidadPorEje", cantidadPorEjes);
            _datosRender.ComputeShader().SetInt("cantidadIndices", cantidadIndices);
            _datosRender.ComputeShader().SetFloats("isoLevel", _datosRender.IsoLevel());

            _datosRender.ComputeShader().Dispatch(kernel, cantidadPorEjes, cantidadPorEjes, cantidadPorEjes);
        }

        private void PasarInformacion()
        {
            ComputeBuffer triangulosBuffer = _bufferTriangulos.Buffer;
            int[] args = new int[] { 0, 1, 0, 0 };
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(triangulosBuffer, _argBuffer, 0);

            Plane[] planos = GeometryUtility.CalculateFrustumPlanes(_camara);
            _planosBuffer.SetData(planos);

            _material.SetPass(0);
            _material.SetBuffer("triangulos", triangulosBuffer);
            _material.SetBuffer("planos", _planosBuffer);
        }

        private void Render()
        {
            Graphics.DrawProceduralIndirect(_material, _generador.MarchingCubeMesh.Limites, MeshTopology.Points, _argBuffer);
        }
    }
}
