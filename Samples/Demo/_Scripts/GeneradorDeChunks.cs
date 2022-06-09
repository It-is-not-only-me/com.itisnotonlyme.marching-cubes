using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class GeneradorDeChunks : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _radio;
    [SerializeField] private Vector3Int _dimensionDeChunk;
    [SerializeField] private int _cantidadDeChunkPorFrame = 10;

    private IEnumerator Start()
    {
        int contador = 0;
        for (int i = 0; i < _radio; i++)
        {
            for (int k = 0; k < _radio; k++)
            {
                GameObject chunk = Instantiate(_prefab, transform);
                GeneradorDatosPerlin generador = chunk.GetComponent<GeneradorDatosPerlin>();

                Vector3 posicion = transform.position + Vector3.right * (i - 1 / 2) * _dimensionDeChunk.x + Vector3.forward * (k - 1 / 2) * _dimensionDeChunk.z;
                generador.Inicializar(posicion, _dimensionDeChunk);

                if (contador >= _cantidadDeChunkPorFrame)
                {
                    contador = 0;
                    yield return null;
                }

                contador++;
            }

        }
    }
}
