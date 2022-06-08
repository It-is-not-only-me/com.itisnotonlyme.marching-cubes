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
    [SerializeField] private bool _actualizar = true;

    public override Vector3Int Dimension => new Vector3Int((int)_tamanioX, (int)_tamanioY, (int)_tamanioZ) + Vector3Int.one;
    private Vector3 _posicion => transform.position;
    public override bool Actualizar 
    { 
        get 
        {
            if (_actualizar)
            {
                _actualizar = false;
                return true;
            }
            return false;
        }
    }

    public void Inicializar(Vector3 posicion, Vector3Int dimension)
    {
        transform.position = posicion;
        _tamanioX = (uint)dimension.x;
        _tamanioY = (uint)dimension.y;
        _tamanioZ = (uint)dimension.z;
        _actualizar = true;
    }

    public override IEnumerable<Dato> GetDatos()
    {
        Dato dato = new Dato(Vector3.zero, 0);

        for (int i = 0; i < _tamanioX + 1; i++)
            for (int j = 0; j < _tamanioY + 1; j++)
                for (int k = 0; k < _tamanioZ + 1; k++)
                {
                    Vector3 posicion = new Vector3(i, j, k) + _posicion - Dimension / 2;
                    float valorPerlin = Mathf.PerlinNoise(posicion.x * _noiseScale + 200, posicion.z * _noiseScale + 200);
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
