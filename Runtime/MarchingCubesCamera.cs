using System;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [AddComponentMenu("Marching cubes/Camara manager")]
    public class MarchingCubesCamera : MonoBehaviour
    {
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private Material _material;
        [SerializeField] private ObtenerDatosSO _datos;

        [Space]

        [SerializeField] [Range(0, 1)] private float _isoLevel;

        private ComputeBuffer _triangulosBuffer, _datosBuffer, _argBuffer;
        private Vector3Int _dimensiones;

        private void OnPostRender()
        {
            _dimensiones = _datos.GetDimensiones();
            bool sePudoCrear = CrearBuffers();
            if (!sePudoCrear)
                return;

            CargarDatos();

            SetearBuffers();

            Render();

            DestruirBuffers();
        }

        private bool CrearBuffers()
        {
            int datosCount = 1;
            for (int i = 0; i < 3; i++)
                datosCount *= _dimensiones[i];

            if (datosCount <= 1)
                return false;

            int datosStride = 3 * sizeof(int) + 1 * sizeof(float);

            // 5 es la cantidad de triangulos maximos que va a tener un cube segun el algoritmo
            int triangulosCount = datosCount * 5;
            int trianguloStride = 4 * ( 3 * sizeof(float) );

            int argCount = 5;
            int argStride = sizeof(int);

            _datosBuffer = new ComputeBuffer(datosCount, datosStride);
            _triangulosBuffer = new ComputeBuffer(triangulosCount, trianguloStride, ComputeBufferType.Append);
            _argBuffer = new ComputeBuffer(argCount, argStride, ComputeBufferType.IndirectArguments);

            return true;
        }

        private void CargarDatos()
        {
            int cantidadDeDatos = 1;
            for (int i = 0; i < 3; i++)
                cantidadDeDatos *= _dimensiones[i];
            Dato[] datos = new Dato[cantidadDeDatos];

            int j = 0;
            foreach (Dato dato in _datos.GetDatos())
            {
                datos[j] = dato;
                j++;
            }

            _datosBuffer.SetData(datos);
        }

        private void SetearBuffers()
        {
            int kernel = _computeShader.FindKernel("March");

            _computeShader.SetBuffer(kernel, "triangles", _triangulosBuffer);
            _computeShader.SetBuffer(kernel, "datos", _datosBuffer);
            _computeShader.SetFloat(kernel, _isoLevel);
            _computeShader.SetInts(kernel, _dimensiones.x, _dimensiones.y, _dimensiones.z);

            _computeShader.Dispatch(kernel, _dimensiones.x, _dimensiones.y, _dimensiones.z);

            int[] args = new int[] { 0, 1, 0, 0, 0};
            _argBuffer.SetData(args);
            ComputeBuffer.CopyCount(_triangulosBuffer, _argBuffer, 0);
            _argBuffer.GetData(args);
        }

        private void Render()
        {
            _material.SetPass(0);
            _material.SetBuffer("triangulos", _triangulosBuffer);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer, 0);
        }

        private void DestruirBuffers()
        {
            _triangulosBuffer.Dispose();
            _datosBuffer.Dispose();
            _argBuffer.Dispose();
        }
    }
}
