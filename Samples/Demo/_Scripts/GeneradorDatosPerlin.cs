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
            _actualizar &= false;
            return necesitaActualizar; 
        }
    }

    public override Bounds Bounds => _bounds;

    private uint _tamanioX, _tamanioY, _tamanioZ;
    private Dato[] _datos;
    private float _noiseScale = 0.05f;
    private Bounds _bounds;

    public void Inicializar(Vector3 posicion, Vector3 ancho, Vector3Int puntoPorEje)
    {
        transform.position = posicion;
        _tamanioX = (uint)puntoPorEje.x;
        _tamanioY = (uint)puntoPorEje.y;
        _tamanioZ = (uint)puntoPorEje.z;
        _actualizar = true;

        _bounds = new Bounds(posicion, ancho);
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
                    float x = Mathf.Lerp(0, _bounds.size.x * 2, ((float)i) / (puntosPorEje.x - 1));
                    float y = Mathf.Lerp(0, _bounds.size.y * 2, ((float)j) / (puntosPorEje.y - 1));
                    float z = Mathf.Lerp(0, _bounds.size.z * 2, ((float)k) / (puntosPorEje.z - 1));

                    Vector3 posicion = new Vector3(x, y, z) + _bounds.center - _bounds.size;
                    float valorPerlin = Mathf.PerlinNoise(posicion.x * _noiseScale + 200, posicion.z * _noiseScale + 200);
                    float valor = valorPerlin * _tamanioY - j;
                    _datos[contador++].CargarDatos(posicion, valor);
                }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_bounds.center, _bounds.size * 2);
    }
}
