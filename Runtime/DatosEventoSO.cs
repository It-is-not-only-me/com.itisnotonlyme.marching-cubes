using System;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "Pasar datos", menuName = "Evento/Marching Cubes/Evento de pasar datos")]
    public class DatosEventoSO : ScriptableObject
    {
        public Action<IObtenerDatos> ObtenerDatosEvento;

        public void Invoke(IObtenerDatos datos)
        {
            ObtenerDatosEvento?.Invoke(datos);
        }
    }
}