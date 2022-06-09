using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "Datos para render", menuName = "MarchingCubes/Datos para render")]
    public class DatosRender : ScriptableObject
    {
        public Shader GeometryShader;
        public ComputeShader ComputeShader;
        [Range(0, 1)] public float IsoLevel;
    }
}