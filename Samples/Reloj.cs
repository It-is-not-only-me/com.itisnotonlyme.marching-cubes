using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class Reloj : MonoBehaviour
{
    [SerializeField] private DatosEventoSO _evento;
    [SerializeField] private List<ObtenerDatosSO> _datos;

    private void Update()
    {
        foreach (ObtenerDatosSO datos in _datos)
            _evento?.Invoke(datos);
    }
}
