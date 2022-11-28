using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct MarchingCubeMesh
    {
        private Bounds _limites;
        private Dato[] _datos;
        private int[] _indices;
        private Vector4[] _uvs;

        public MarchingCubeMesh(Bounds limites,
                                Dato[] datos = null,
                                int[] indices = null,
                                Vector4[] uvs = null)
        {
            _limites = limites;
            _datos = datos;
            _indices = indices;
            _uvs = uvs;
        }

        public Bounds Limites
        {
            get => _limites;
        }

        public Dato[] Datos
        {
            get
            {
                if (_datos == null)
                    _datos = new Dato[Indices.Length];
                return _datos;
            }
            set
            {
                _datos = value;
            }
        }

        public int[] Indices
        {
            get
            {
                return _indices;
            }
            set
            {
                _indices = value;
            }

        }

        public Vector4[] Uvs
        {
            get
            {
                if (_uvs == null)
                    _uvs = new Vector4[Datos.Length];
                return _uvs;
            }
            set
            {
                _uvs = value;
            }
        }
    }
}
