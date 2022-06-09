using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct Dato
    {
        public Vector3 Posicion;
        public Vector2 Uv;
        public float Valor;

        public Dato(Vector3 posicion, float valor, Vector2 uv)
        {
            Posicion = posicion;
            Valor = valor;
            Uv = uv;
        }

        public Dato(Vector3 posicion, float valor)
            : this(posicion, valor, Vector2.zero)
        {
        }

        public void CargarDatos(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }
    }
}
