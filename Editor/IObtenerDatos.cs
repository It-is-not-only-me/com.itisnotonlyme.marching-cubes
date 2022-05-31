using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IObtenerDatos
    {
        public Vector3Int Dimension { get; }

        public int Id { get; }

        public IEnumerable<Dato> GetDatos();
    }
}
