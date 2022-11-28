using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;


public class GeneradorDatosPerlin : GenerarDatos
{
    [SerializeField] private bool _actualizar = true;

    private Vector3Int _numeroDePuntosPorEje => new Vector3Int((int)_tamanioX, (int)_tamanioY, (int)_tamanioZ) + Vector3Int.one;
    public override bool Actualizar
    {
        get
        {
            bool necesitaActualizar = _actualizar;
            _actualizar &= false;
            return necesitaActualizar;
        }
    }

    public override MarchingCubeMesh MarchingCubeMesh => _marchingCubeMesh;

    private uint _tamanioX, _tamanioY, _tamanioZ;
    private float _noiseScale = 0.05f;

    private MarchingCubeMesh _marchingCubeMesh;
    private Bounds _limites;

    public void Inicializar(Vector3 posicion, Vector3 ancho, Vector3Int puntosPorEje)
    {
        transform.position = Vector3.zero;
        _tamanioX = (uint)puntosPorEje.x;
        _tamanioY = (uint)puntosPorEje.y;
        _tamanioZ = (uint)puntosPorEje.z;
        _actualizar = true;

        _limites = new Bounds(posicion, ancho * 2);

        int cantidadDeDatos = _numeroDePuntosPorEje.x * _numeroDePuntosPorEje.y * _numeroDePuntosPorEje.z;
        int cantidadDeindices = ((_numeroDePuntosPorEje.x - 1) * (_numeroDePuntosPorEje.y - 1) * (_numeroDePuntosPorEje.z - 1)) * 8;

        _marchingCubeMesh = new MarchingCubeMesh(_limites, cantidadDeDatos, cantidadDeindices);
        GenerarMesh();
    }

    private void GenerarMesh()
    {
        for (int i = 0, contador = 0; i < _numeroDePuntosPorEje.x; i++)
            for (int j = 0; j < _numeroDePuntosPorEje.y; j++)
                for (int k = 0; k < _numeroDePuntosPorEje.z; k++, contador++)
                {
                    float x = Mathf.Lerp(0, _limites.size.x, ((float)i) / (_numeroDePuntosPorEje.x - 1));
                    float y = Mathf.Lerp(0, _limites.size.y, ((float)j) / (_numeroDePuntosPorEje.y - 1));
                    float z = Mathf.Lerp(0, _limites.size.z, ((float)k) / (_numeroDePuntosPorEje.z - 1));

                    Vector3 posicion = new Vector3(x, y, z) + _limites.center - (_limites.size / 2);
                    Vector3 posicionPerlin = posicion * _noiseScale + Vector3.one * 200;
                    float valor = Perlin3D(posicionPerlin);

                    _marchingCubeMesh.AgregarUV(new Color(i % 2, j % 2, 0, 0), contador);
                    _marchingCubeMesh.AgregarDato(new Color(posicion.x, posicion.y, posicion.z, valor), contador);
                }

        for (int i = 0, posicion = 0; i < _numeroDePuntosPorEje.x - 1; i++)
            for (int j = 0; j < _numeroDePuntosPorEje.y - 1; j++)
                for (int k = 0; k < _numeroDePuntosPorEje.z - 1; k++, posicion += 8)
                {
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i, j, k, _numeroDePuntosPorEje), posicion);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i + 1, j, k, _numeroDePuntosPorEje), posicion + 1);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i + 1, j, k + 1, _numeroDePuntosPorEje), posicion + 2);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i, j, k + 1, _numeroDePuntosPorEje), posicion + 3);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i, j + 1, k, _numeroDePuntosPorEje), posicion + 4);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i + 1, j + 1, k, _numeroDePuntosPorEje), posicion + 5);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i + 1, j + 1, k + 1, _numeroDePuntosPorEje), posicion + 6);
                    _marchingCubeMesh.AgregarIndice(IndicePorEje(i, j + 1, k + 1, _numeroDePuntosPorEje), posicion + 7);
                }
    }

    private int IndicePorEje(int x, int y, int z, Vector3Int puntosPorEje)
    {
        return z + y * puntosPorEje.z + x * puntosPorEje.z * puntosPorEje.y;
    }

    private float Perlin3D(Vector3 posicion)
    {
        float x = posicion.x, y = posicion.y, z = posicion.z;

        float XY = Mathf.PerlinNoise(x, y);
        float YZ = Mathf.PerlinNoise(y, z);
        float ZX = Mathf.PerlinNoise(z, x);

        float YX = Mathf.PerlinNoise(y, z);
        float ZY = Mathf.PerlinNoise(z, y);
        float XZ = Mathf.PerlinNoise(x, z);

        return (XY + YZ + ZX + YX + ZY + XZ) / 6f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_limites.center, _limites.size);
    }
}
