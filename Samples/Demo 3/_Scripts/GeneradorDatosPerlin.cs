    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class GeneradorDatosPerlin : GenerarDatos
{
    [SerializeField] private uint _tamanioX, _tamanioY, _tamanioZ;

    [Space]

    [SerializeField] private float _velocidad;
    [SerializeField] private float _noiseScale;
    private float _desfase = 0f;
    private bool _actualizarse = true;

    public override Vector3Int Dimension => new Vector3Int((int)_tamanioX, (int)_tamanioY, (int)_tamanioZ) + Vector3Int.one;
    private Vector3 _posicion => transform.position;
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

        for (int i = 0; i < _tamanioX + 1; i++)
            for (int j = 0; j < _tamanioY + 1; j++)
                for (int k = 0; k < _tamanioZ + 1; k++)
                {
                    Vector3 posicion = new Vector3(i, j, k) + _posicion - Dimension / 2;
                    float valorPerlin = Mathf.PerlinNoise(i * _noiseScale + _desfase, k * _noiseScale);
                    float valor = valorPerlin * _tamanioY - j;
                    dato.CargarDatos(posicion, valor);
                    yield return dato;
                }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_posicion, Dimension - Vector3Int.one);
    }
}
