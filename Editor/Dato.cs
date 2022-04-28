using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class Dato
    {
        public Vector3 Posicion;
        public float Valor;

        public Dato(Vector3 posicion, float valor)
        {
            CargarDatos(posicion, valor);
        }

        public Dato()
            : this(Vector3.zero, 0)
        {
        }

        public void CargarDatos(Vector3 posicion, float valor)
        {
            Posicion = posicion;
            Valor = valor;
        }
    }
}
