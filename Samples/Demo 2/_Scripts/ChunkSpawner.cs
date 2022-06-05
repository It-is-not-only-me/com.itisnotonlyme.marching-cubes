using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    [SerializeField] private Transform _centro;
    [SerializeField] private GameObject _chunkPrefab;

    [Space]

    [SerializeField] private Vector3Int _radio;
    [SerializeField][Range(5, 20)] int _lodRango;

    private Vector3 _posicion => _centro.position;
    private Vector3 _posicionAnterior;
    private Coroutine _rutinaActual;

    [Space]


    [SerializeField] private float _distanciaActualizacion = 10f;

    private struct InfoChunk
    {
        public Vector3 Posicion;
        public Vector3Int Dimensiones;
        public int Lod;

        public InfoChunk(Vector3 posicion, Vector3Int dimensiones, int lod)
        {
            Posicion = posicion;
            Dimensiones = dimensiones;
            Lod = lod;
        }
    }

    private void Start()
    {
        _posicionAnterior = _posicion;
        _rutinaActual = StartCoroutine(Actualizar());
    }

    private void Update()
    {
        if (Vector3.Distance(_posicion, _posicionAnterior) < _distanciaActualizacion)
            return;

        _posicionAnterior = _posicion;

        if (_rutinaActual != null)
            StopCoroutine(_rutinaActual);
        _rutinaActual = StartCoroutine(Actualizar());
    }

    private IEnumerator Actualizar()
    {
        for (int radio = 0, lod = 1; radio < ComponenteMenor(_radio); radio += _lodRango, lod++)
        {
            foreach (InfoChunk infoChunk in InfoChunksSegunLOD(lod))
                CrearChunk(infoChunk);
            yield return null;
        }
    }

    private int ComponenteMenor(Vector3Int vector)
    {
        return (vector.x <= vector.z) ? vector.x : vector.z;
        /*if (vector.x <= vector.y && vector.x <= vector.z)
            return vector.x;

        return (vector.y <= vector.z) ? vector.y : vector.z;*/
    }

    private void CrearChunk(InfoChunk infoChunk)
    {
        GameObject chunk = Instantiate(_chunkPrefab);
        ChunkBehaviour comportamiento = chunk.GetComponent<ChunkBehaviour>();
        comportamiento.Inicializar(infoChunk.Posicion, infoChunk.Dimensiones, infoChunk.Lod);
        chunk.transform.parent = transform;
    }

    private List<InfoChunk> InfoChunksSegunLOD(int lod)
    {
        if (lod < 1)
            return null;

        if (lod == 1)
        {
            Vector3Int dimensiones = new Vector3Int(_lodRango, _radio.y, _lodRango) + Vector3Int.one;
            return new List<InfoChunk> { new InfoChunk(_posicion, dimensiones, lod) };
        }

        List<InfoChunk> listaInfo = new List<InfoChunk>();

        for (int i = -1; i <= 1; i += 2)
            for (int j = -1; j <= 1; j += 2)
            {
                float distanciaDelCentro = Mathf.FloorToInt(_lodRango * (lod - 0.5f));
                float translacion = Mathf.FloorToInt(_lodRango * 0.5f);

                float posicionX = (j < 0) ? translacion : distanciaDelCentro;
                float posicionZ = (j < 0) ? distanciaDelCentro : translacion;

                Vector3 posicion = new Vector3(posicionX * i * j, 0, posicionZ * i * -1) + _posicion;

                int ancho = Mathf.CeilToInt(_lodRango * 0.5f);
                int largo = Mathf.CeilToInt(_lodRango * (lod - 0.5f));

                int dimensionX = (j < 0) ? largo : ancho;
                int dimensionZ = (j < 0) ? ancho : largo;

                Vector3Int dimension = new Vector3Int(dimensionX, _radio.y, dimensionZ) + Vector3Int.one;

                listaInfo.Add(new InfoChunk(posicion, dimension, 1));
            }

        return listaInfo;
    }
}
