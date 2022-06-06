using System;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    [CreateAssetMenu(fileName = "Pasar datos", menuName = "MarchingCubes/Eventos/Evento de pasar datos")]
    public class DatosEventoSO : ScriptableObject
    {
        public Action<IObtenerDatos> ObtenerDatosEvento;

        public void Invoke(IObtenerDatos datos)
        {
            ObtenerDatosEvento?.Invoke(datos);
        }
    }
}