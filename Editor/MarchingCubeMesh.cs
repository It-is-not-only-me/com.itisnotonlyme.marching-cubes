using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct MarchingCubeMesh
    {
        public Bounds Limites;
        public Dato[] Datos;
        public int Indices;
        public Vector2[] Uv;
        public Vector2[] Uv2;
        public Color[] Colores;

        public MarchingCubeMesh(Bounds limites,
                                Dato[] datos,
                                int indices,
                                Vector2[] uv,
                                Vector2[] uv2,
                                Color[] colores)
        {
            Limites = limites;
            Datos = datos;
            Indices = indices;
            Uv = uv;
            Uv2 = uv2;
            Colores = colores;
        }
    }
}
