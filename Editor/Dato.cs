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
            int esIgual = 0;

            esIgual += CompararFloat(Valor, dato.Valor);
            for (int i = 0; i < 3; i++)
                esIgual += CompararFloat(Posicion[i], dato.Posicion[i]);

            return esIgual;
        }

        private static int CompararFloat(float valor1, float valor2)
        {
            return (int) (_aplificacion * (valor1 - valor2));
        }
    }
}
