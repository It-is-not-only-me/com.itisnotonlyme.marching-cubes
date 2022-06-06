using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class GeneradorDatosPerlin : GenerarDatos
{
    [SerializeField] private uint _tamanioX, _tamanioY, _tamanioZ;
    [SerializeField] private float _scale;
    [SerializeField] private Vector3 _corrimiento;

    [Space]

    [SerializeField] private float _velocidad;
    [SerializeField] private float _noiseScale;
    private float _desfase = 0f;
    private bool _actualizarse = true;

    public override Vector3Int Dimension => new Vector3Int((int)_tamanioX, (int)_tamanioY, (int)_tamanioZ);
    public override Vector3 Posicion => transform.position + _corrimiento;
    public override bool Actualizar 
    { 
        get 
        {
            if (_actualizarse)
            {
                _actualizarse = false;
                return true;
            }
            return false;
        }
    }

    public override IEnumerable<Dato> GetDatos()
    {
        Dato dato = new Dato(Vector3.zero, 0);
        _desfase += _velocidad;
        float noiseScale = 0.05f;
        for (int i = 0; i < _tamanioX; i++)
            for (int j = 0; j < _tamanioY; j++)
                for (int k = 0; k < _tamanioZ; k++)
                {
                    Vector3 posicion = new Vector3(i * _scale, j * _scale, k * _scale) + _corrimiento;
                    float valorPerlin = Mathf.PerlinNoise(i * noiseScale + _desfase, k * noiseScale);
                    float valor = valorPerlin * _tamanioY - j;
                    dato.CargarDatos(posicion, valor);
                    yield return dato;
                }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Posicion, Dimension * 2);
    }
}
