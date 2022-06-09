using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class GeneradorDeChunks : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    [Space]

    [SerializeField] private int _radio;
    [SerializeField] private Vector3 _dimensionDeChunk;
    [SerializeField] private Vector3Int _puntosPorEje;

    [Space]

    [SerializeField] private int _cantidadDeChunkPorFrame = 10;
    [SerializeField] private DatosRender _datosRender;
    [SerializeField] private bool _meshLocal;

    private IEnumerator Start()
    {
        int contador = 0;
        for (int i = 0; i < _radio; i++)
        {
            for (int k = 0; k < _radio; k++)
            {
                GameObject chunk = Instantiate(_prefab, transform);
                GeneradorDatosPerlin generador = chunk.GetComponent<GeneradorDatosPerlin>();

                Vector3 posicion = transform.position + Vector3.right * (i - 1 / 2) * _dimensionDeChunk.x * 2 + Vector3.forward * (k - 1 / 2) * _dimensionDeChunk.z * 2;
                
                generador.Inicializar(posicion, _dimensionDeChunk, _puntosPorEje);

                if (contador >= _cantidadDeChunkPorFrame)
                {
                    contador = 0;
                    yield return null;
                }

                if (_meshLocal)
                    AgregarMeshLocal(chunk, generador);

                contador++;
            }
        }
    }

    private void AgregarMeshLocal(GameObject chunk, GeneradorDatosPerlin generador)
    {
        chunk.AddComponent<MeshFilter>();
        MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
        meshFilter.mesh = MarchingCubes.CrearMesh(generador, _datosRender);

        chunk.AddComponent<MeshCollider>();

        MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;
    }
}
