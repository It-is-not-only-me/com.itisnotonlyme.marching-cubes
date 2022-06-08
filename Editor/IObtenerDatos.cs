using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IObtenerDatos
    {
        public Bounds Bounds { get; }

        public Vector3Int NumeroDePuntosPorEje { get; }

        public Dato[] GetDatos();
    }
}
