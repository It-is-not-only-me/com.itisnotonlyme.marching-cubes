using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct Dato
    {
        public Vector3 Posicion;
        public float Valor;
        public Vector2 UV;
        public Vector4 TexCoor;

        public Dato(Vector3 posicion, float valor, Vector2 uV, Vector4 texCoor)
        {
            Posicion = posicion;
            Valor = valor;
            UV = uV;
            TexCoor = texCoor;
        }

        public Dato(Vector3 posicion, float valor)
            : this(posicion, valor, Vector2.zero, Vector4.zero)
        {
        }

        public void CargarDatos(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }
    }
}
