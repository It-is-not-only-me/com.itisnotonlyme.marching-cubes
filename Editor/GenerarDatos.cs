using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class GenerarDatos : MonoBehaviour, IObtenerDatos
    {
        public abstract Bounds Bounds { get; }
        public abstract Vector3Int NumeroDePuntosPorEje { get; }
        public abstract bool Actualizar { get; }
        public abstract Dato[] GetDatos();
    }
}
