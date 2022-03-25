using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IObtenerDatos
    {
        public Vector3Int GetDimensiones();

        public IEnumerable<Dato> GetDatos();
    }
}
