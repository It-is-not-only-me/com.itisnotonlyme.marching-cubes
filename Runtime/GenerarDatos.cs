using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public abstract class GenerarDatos : MonoBehaviour, IObtenerDatos
    {
        public int Id => GetInstanceID();

        public virtual bool Actualizar { get; protected set; }

        public abstract Vector3Int Dimension { get; }

        public abstract IEnumerable<Dato> GetDatos();
    }
}
