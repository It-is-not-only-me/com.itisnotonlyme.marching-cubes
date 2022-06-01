using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class ChunkBehaviour : MonoBehaviour, IObtenerDatos
{
    [SerializeField] private DatosEventoSO _agregarChunk, _actualizarChunk, _sacarChunk;
    [SerializeField] private float _noiseScale = 0.05f;

    [Space]

    [SerializeField] private Vector3Int _dimension;
    private int _lod = 1;

    public Vector3Int Dimension => _dimension / _lod;
    public int Id => GetInstanceID();

    private void Start()
    {
        if (_agregarChunk != null)
            _agregarChunk.Invoke(this);
    }

    public IEnumerable<Dato> GetDatos()
    {
        Dato dato = new Dato(Vector3.zero, 0);
        for (int i = 0; i < Dimension.x; i++)
            for (int j = 0; j < Dimension.y; j++)
                for (int k = 0; k < Dimension.z; k++)
                {
                    Vector3 posicion = new Vector3(i, j, k) * _lod + transform.position;
                    float valorPerlin = Mathf.PerlinNoise(posicion.x * _noiseScale, posicion.z * _noiseScale);
                    float valor = valorPerlin * 20 - j + Dimension.y / 2;
                    //Vector3 posicionNoise = posicion * _noiceScale;
                    //float valor = PerlinNoise3D(posicionNoise.x, posicionNoise.y, posicionNoise.z);
                    dato.CargarDatos(posicion, valor);
                    yield return dato;
                }
    }

    public void Inicializar(Vector3 posicion, Vector3Int dimension, int lod)
    {
        _lod = lod;
        transform.position = posicion - dimension;
        _dimension = dimension * 2;
        Actualizar();
    }

    private void Actualizar()
    {
        if (_actualizarChunk != null)
            _actualizarChunk.Invoke(this);
    }

    private float PerlinNoise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
