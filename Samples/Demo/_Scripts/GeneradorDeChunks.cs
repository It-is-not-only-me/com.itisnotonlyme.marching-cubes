using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class GeneradorDeChunks : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _radio;
    [SerializeField] private Vector3Int _dimensionDeChunk;

    private IEnumerator Start()
    {
        int contador = 0;
        for (int i = 0; i < _radio; i++)
        {
            for (int k = 0; k < _radio; k++)
            {
                GameObject chunk = Instantiate(_prefab, transform);
                GeneradorDatosPerlin generador = chunk.GetComponent<GeneradorDatosPerlin>();

                Vector3 posicion = transform.position + Vector3.right * i * _dimensionDeChunk.x / 2 + Vector3.forward * k * _dimensionDeChunk.z / 2;
                generador.Inicializar(posicion, _dimensionDeChunk);

                if (contador >= 10)
                {
                    contador = 0;
                    yield return null;
                }

                contador++;
            }

        }
    }
}
