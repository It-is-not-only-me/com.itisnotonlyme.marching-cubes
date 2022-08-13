using System;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct Dato : IComparable
    {
        private static float _aplificacion = 1000f;

        public Vector3 Posicion;
        public float Valor;

        public Dato(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }

        public void CargarDatos(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }

        public int CompareTo(object obj)
        {
            Dato dato = (Dato)obj;
            return (int)(_aplificacion * (Valor - dato.Valor));
        }
    }
}
