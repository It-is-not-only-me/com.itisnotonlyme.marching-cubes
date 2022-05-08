using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class ObtenerDatosSO : ScriptableObject, IObtenerDatos
    {
        public abstract Vector3Int Dimension { get; }
        public abstract int Cantidad { get; }
        public abstract IEnumerable<Dato> GetDatos();
    }
}
