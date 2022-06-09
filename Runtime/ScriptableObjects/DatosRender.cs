using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "Datos para render", menuName = "MarchingCubes/Datos para render")]
    public class DatosRender : ScriptableObject, IDatoRender
    {
        [SerializeField] private Shader _geometryShader;
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] [Range(0, 1)] private float _isoLevel;

        public Shader GeometryShader() => _geometryShader;
        public ComputeShader ComputeShader() => _computeShader;
        public float IsoLevel() => _isoLevel;
    }
}