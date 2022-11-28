using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "_datos para render", menuName = "MarchingCubes/_datos para render")]
    public class DatosRender : ScriptableObject, IDatoRender
    {
        [SerializeField] private Shader _geometryShader;
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private Camera _camara;
        [SerializeField] [Range(0, 1)] private float _isoLevel;

        public Shader GeometryShader() => _geometryShader;
        public ComputeShader ComputeShader() => _computeShader;
        public Camera Camara() => _camara;
        public float IsoLevel() => _isoLevel;
    }
}