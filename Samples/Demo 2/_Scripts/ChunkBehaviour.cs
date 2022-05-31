using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class ChunkBehaviour : MonoBehaviour, IObtenerDatos
{
    [SerializeField] private DatosEventoSO _agregarChunk, _actualizarChunk, _sacarChunk;
    [SerializeField] private float _noiceScale;

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
        float noiseScale = 0.05f;
        for (int i = 0; i < Dimension.x; i++)
            for (int j = 0; j < Dimension.y; j++)
                for (int k = 0; k < Dimension.z; k++)
                {
                    Vector3 posicion = new Vector3(i, j, k) * _lod + transform.position;
                    float valorPerlin = Mathf.PerlinNoise(posicion.x * noiseScale, posicion.z * noiseScale);
                    float valor = valorPerlin * 5 - j;
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
}
