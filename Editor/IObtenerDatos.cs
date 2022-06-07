using System.Collections.Generic;
using UnityEngine;

/*
 * IObtenerDatos tiene que decir cuantos puntos en cada eje va a mandar
 * La cantidad total no es necesario
 * 
 */
namespace ItIsNotOnlyMe.MarchingCubes
{
    public interface IObtenerDatos
    {
        public Vector3Int Dimension { get; }

        public int Id { get; }

        public IEnumerable<Dato> GetDatos();
    }
}
