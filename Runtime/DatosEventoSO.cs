using System;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "Evento", menuName = "Ejemplo marching cubes/Evento de datos")]
    public class DatosEventoSO : ScriptableObject
    {
        public Action<IObtenerDatos> ObtenerDatosEvento;

        public void Invoke(IObtenerDatos datos)
        {
            ObtenerDatosEvento?.Invoke(datos);
        }
    }
}