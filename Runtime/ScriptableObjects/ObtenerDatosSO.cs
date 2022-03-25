using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class ObtenerDatosSO : ScriptableObject, IObtenerDatos
    {
        public abstract Vector3Int GetDimensiones();
        public abstract IEnumerable<Dato> GetDatos();
    }
}
