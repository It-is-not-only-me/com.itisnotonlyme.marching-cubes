using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct Dato
    {
        public Vector3Int Posicion;
        public float Valor;

        public Dato(Vector3Int posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }
    }
}
