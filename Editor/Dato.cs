using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct Dato
    {
        public Vector3 Posicion;
        public float Valor;

        public Dato(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }
    }
}
