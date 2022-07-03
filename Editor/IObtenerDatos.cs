using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IObtenerDatos
    {
        public MarchingCubeMesh MarchingCubeMesh { get; }

        public Vector3Int NumeroDePuntosPorEje { get; }
    }
}
