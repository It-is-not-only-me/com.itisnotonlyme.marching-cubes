using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class GenerarDatos : MonoBehaviour, IObtenerDatos
    {
        public abstract MarchingCubeMesh MarchingCubeMesh { get; }
        public abstract bool Actualizar { get; }
    }
}
