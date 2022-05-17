using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class Reloj : MonoBehaviour
{
    [System.Serializable]
    private struct MandarInfo
    {
        public ObtenerDatosEjemplo Dato;
        public bool MandarDatos;
    }

    [SerializeField] private DatosEventoSO _eventoMandar, _actualizarDatos;
    [SerializeField] private List<MandarInfo> _datos;

    private void Start()
    {
        foreach (MandarInfo datos in _datos)
            if (datos.MandarDatos)
                _eventoMandar?.Invoke(datos.Dato);
    }

    private void Update()
    {
        foreach (MandarInfo datos in _datos)
            if (datos.MandarDatos)
                _actualizarDatos?.Invoke(datos.Dato);
    }
}
