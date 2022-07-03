using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class GenerarDatos : MonoBehaviour, IObtenerDatos
    {
        public abstract MarchingCubeMesh MarchingCubeMesh { get; }
        public abstract Vector3Int NumeroDePuntosPorEje { get; }
        public abstract bool Actualizar { get; }
    }
}
