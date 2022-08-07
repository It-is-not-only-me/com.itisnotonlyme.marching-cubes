using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IDatoRender
    {
        public Shader GeometryShader();
        public ComputeShader ComputeShader();
        public Camera Camara();
        public float IsoLevel();
    }
}