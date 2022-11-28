using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public struct MarchingCubeMesh
    {
        private Bounds _limites;
        private Dato[] _datos;
        private int[] _indices;
        private Vector2[] _uv;
        private Vector2[] _uv2;

        public MarchingCubeMesh(Bounds limites,
                                Dato[] datos = null,
                                int[] indices = null,
                                Vector2[] uv = null,
                                Vector2[] uv2 = null)
        {
            _limites = limites;
            _datos = datos;
            _indices = indices;
            _uv = uv;
            _uv2 = uv2;
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

        public Vector2[] Uvs
        {
            get
            {
                if (_uv == null)
                    _uv = new Vector2[Datos.Length];
                return _uv;
            }
            set
            {
                _uv = value;
            }
        }

        public Vector2[] Uvs2
        {
            get
            {
                if (_uv2 == null)
                    _uv2 = new Vector2[Datos.Length];
                return _uv2;
            }
            set
            {
                _uv2 = value; 
            }
        }
    }
}
