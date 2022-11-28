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
        private BufferManager _bufferDatos, _bufferTriangulos, _bufferIndices, _bufferUvs, _bufferUvs2, _bufferColores;
        private bool _usaUVs, _usaUV2s, _usaColores;
        private GenerarDatos _generador;
        private Camera _camara;

        private int _cantidadDeFloatDatos = 4;
        private int _cantidadDeFloatTriangulos = 3 * 3 + 2 * 3 + 2 * 3 + 3 * 3;
        private int _triangulosPorDato = 5;
        private int _cantidadDeFloatUvs = 2;
        private int _cantidadDeFloatColores = 4;

        private void Awake()
        {
            Material nuevoMaterial = new Material(_datosRender.GeometryShader());
            nuevoMaterial?.CopyPropertiesFromMaterial(_material);
            _material = nuevoMaterial;
            CrearBufferAuxileares();

            _camara = _datosRender.Camara();
            if (_camara == null)
                _camara = Camera.main;

            int datosStride = _cantidadDeFloatDatos * sizeof(float);
            int triangulosStride = _cantidadDeFloatTriangulos * sizeof(float);
            int indicesStride = sizeof(int);
            int uvsStride = _cantidadDeFloatUvs * sizeof(float);
            int coloresStride = _cantidadDeFloatColores * sizeof(float);

            _bufferDatos = new BufferManager(datosStride, CrearBuffer);
            _bufferTriangulos = new BufferManager(triangulosStride, CrearBufferAppend);
            _bufferIndices = new BufferManager(indicesStride, CrearBuffer);
            _bufferUvs = new BufferManager(uvsStride, CrearBuffer);
            _usaUVs = true;
            _bufferUvs2 = new BufferManager(uvsStride, CrearBuffer);
            _usaUV2s = true;
            _bufferColores = new BufferManager(coloresStride, CrearBuffer);
            _usaColores = true;

            if (!TryGetComponent(out _generador))
                Debug.LogError("No hay generador");
        }

        private ComputeBuffer CrearBuffer(int cantidad, int stride) => new ComputeBuffer(cantidad, stride);
        private ComputeBuffer CrearBufferAppend(int cantidad, int stride) => new ComputeBuffer(cantidad, stride, ComputeBufferType.Append);

        private void OnDestroy()
        {
            _argBuffer?.Dispose();
            _planosBuffer?.Dispose();

            _bufferDatos.Destruir();
            _bufferTriangulos.Destruir();
            _bufferIndices.Destruir();
            _bufferUvs.Destruir();
            _bufferUvs2.Destruir();
            _bufferColores.Destruir();
        }

        private void Update()
        {
            if (_generador.Actualizar)
            {
                ActualizarDatos();
                PasarInformacion();
            }
            
            Render();
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
            int cantidadDeDatos = mesh.Datos.Length;

            _bufferDatos.ObtenerBuffer(cantidadDeDatos).SetData(mesh.Datos);

            int triangulosCount = cantidadDeDatos * _triangulosPorDato;
            _bufferTriangulos.ObtenerBuffer(triangulosCount);

            int cantidadDeindices = mesh.Indices.Length;
            _bufferIndices.ObtenerBuffer(cantidadDeindices).SetData(mesh.Indices);

            int cantidadDeUvs = mesh.Uvs.Length;
            _usaUVs = cantidadDeUvs != 0;
            if (_usaUVs)
                _bufferUvs.ObtenerBuffer(cantidadDeUvs).SetData(mesh.Uvs);

            int cantidadDeUvs2 = mesh.Uvs2.Length;
            _usaUV2s = cantidadDeUvs2 != 0;
            if (_usaUV2s)
                _bufferUvs2.ObtenerBuffer(cantidadDeUvs2).SetData(mesh.Uvs2);

            int cantidadDeColores = mesh.Colores.Length;
            _usaColores = cantidadDeColores != 0;
            if (_usaColores)
                _bufferColores.ObtenerBuffer(cantidadDeColores).SetData(mesh.Colores);

            Dispatch();
        }

        private void Dispatch()
        {
            int kernel = _datosRender.ComputeShader().FindKernel("March");

            int cantidadIndices = _bufferIndices.Buffer.count;
            int cantidadPorEjes = Mathf.CeilToInt(Mathf.Pow(cantidadIndices / 8, 1.0f / 3.0f));

            _datosRender.ComputeShader().SetBuffer(kernel, "triangles", _bufferTriangulos.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "datos", _bufferDatos.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "indices", _bufferIndices.Buffer);
            _datosRender.ComputeShader().SetBuffer(kernel, "uvs", _bufferUvs.Buffer);
            _datosRender.ComputeShader().SetBool("usaUVs", _usaUVs);
            _datosRender.ComputeShader().SetBuffer(kernel, "uvs2", _bufferUvs2.Buffer);
            _datosRender.ComputeShader().SetBool("usaUV2s", _usaUV2s);
            _datosRender.ComputeShader().SetBuffer(kernel, "colores", _bufferColores.Buffer);
            _datosRender.ComputeShader().SetBool("usaColores", _usaColores);

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
