using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;


public class GeneradorDatosPerlin : GenerarDatos
{
    [SerializeField] private bool _actualizar = true;

    public override Vector3Int NumeroDePuntosPorEje => new Vector3Int((int)_tamanioX, (int)_tamanioY, (int)_tamanioZ) + Vector3Int.one;
    public override bool Actualizar 
    { 
        get 
        {
            bool necesitaActualizar = _actualizar;
            if (_actualizar) _actualizar = false;
            return necesitaActualizar; 
        }
    }

    public override Bounds Bounds => _bounds;

    private uint _tamanioX, _tamanioY, _tamanioZ;
    private Dato[] _datos;
    private float _noiseScale;
    private Bounds _bounds;

    public void Inicializar(Vector3 posicion, Vector3Int dimension)
    {
        transform.position = posicion;
        _tamanioX = (uint)dimension.x;
        _tamanioY = (uint)dimension.y;
        _tamanioZ = (uint)dimension.z;
        _actualizar = true;
        Vector3 size = NumeroDePuntosPorEje / 2;
        _bounds = new Bounds(transform.position, size);
        GenerarDatos();
    }

    public override Dato[] GetDatos()
    {
        return _datos;
    }

    private void GenerarDatos()
    {
        Vector3Int puntosPorEje = NumeroDePuntosPorEje;
        _datos = new Dato[puntosPorEje.x * puntosPorEje.y * puntosPorEje.z];

        int contador = 0;
        for (int i = 0; i < puntosPorEje.x; i++)
            for (int j = 0; j < puntosPorEje.y; j++)
                for (int k = 0; k < puntosPorEje.z; k++)
                {
                    Vector3 posicion = new Vector3(i, j, k) + _bounds.center - _bounds.size;
                    float valorPerlin = Mathf.PerlinNoise(posicion.x * _noiseScale + 200, posicion.z * _noiseScale + 200);
                    float valor = valorPerlin * _tamanioY - j;
                    _datos[contador].CargarDatos(posicion, valor);
                    contador++;
                }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}
