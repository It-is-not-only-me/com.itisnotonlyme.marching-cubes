using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class MarchingCubeMesh
    {
        public Bounds Limites { get; private set; }
        public Texture2D Datos { get; private set; }
        public Texture2D Uvs  { get; private set; }
        public int[] Indices { get; private set; }
        public int CantidadElementos { get; private set; }
        public int Dimensiones { get; private set; }

        private Vector2Int _posicion;

        public MarchingCubeMesh(Bounds limites, int cantidadElementos, int cantidadIndices)
        {
            Limites = limites;
            Indices = new int[cantidadIndices];
            CantidadElementos = cantidadElementos;
            Dimensiones = Mathf.CeilToInt(Mathf.Sqrt(cantidadElementos));
            Datos = new Texture2D(Dimensiones, Dimensiones);
            Uvs = new Texture2D(Dimensiones, Dimensiones);
        }

        public void AgregarDato(Dato dato, int posicion) => AgregarDato(new Color(dato.Posicion.x, dato.Posicion.y, dato.Posicion.z, dato.Valor), posicion);

        public void AgregarDato(Color dato, int posicion)
        {
            PosicionEnTextura(posicion);
            Datos.SetPixel(_posicion.x, _posicion.y, dato);
        }

        public void AgregarUV(Vector4 uvs, int posicion) => AgregarUV(new Color(uvs.x, uvs.y, uvs.z, uvs.w), posicion);

        public void AgregarUV(Color uvs, int posicion)
        {
            PosicionEnTextura(posicion);
            Uvs.SetPixel(_posicion.x, _posicion.y, uvs);
        }

        public void AgregarIndice(int indice, int posicion) => Indices[posicion] = indice;

        private void PosicionEnTextura(int posicion) => _posicion.Set(Mathf.FloorToInt(posicion / Dimensiones), posicion % Dimensiones);
    }
}
