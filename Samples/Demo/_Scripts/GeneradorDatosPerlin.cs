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

    public void Inicializar(Vector3 posicion, Vector3 ancho, Vector3Int puntoPorEje)
    {
        transform.position = Vector3.zero;
        _tamanioX = (uint)puntoPorEje.x;
        _tamanioY = (uint)puntoPorEje.y;
        _tamanioZ = (uint)puntoPorEje.z;
        _actualizar = true;

        _limites = new Bounds(posicion, ancho * 2);
        _marchingCubeMesh = new MarchingCubeMesh(_limites);
        GenerarMesh();
    }

    private void GenerarMesh()
    {
        Vector3Int puntosPorEje = _numeroDePuntosPorEje;
        int cantidadDeDatos = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;
        Dato[] datos = new Dato[cantidadDeDatos];
        Vector2[] uvs = new Vector2[cantidadDeDatos];
        Color[] colores = new Color[cantidadDeDatos];

        int contador = 0;
        for (int i = 0; i < puntosPorEje.x; i++)
            for (int j = 0; j < puntosPorEje.y; j++)
                for (int k = 0; k < puntosPorEje.z; k++)
                {
                    float x = Mathf.Lerp(0, _limites.size.x, ((float)i) / (puntosPorEje.x - 1));
                    float y = Mathf.Lerp(0, _limites.size.y, ((float)j) / (puntosPorEje.y - 1));
                    float z = Mathf.Lerp(0, _limites.size.z, ((float)k) / (puntosPorEje.z - 1));

                    Vector3 posicion = new Vector3(x, y, z) + _limites.center - (_limites.size / 2);
                    Vector3 posicionPerlin = posicion * _noiseScale + Vector3.one * 200;
                    float valor = Perlin3D(posicionPerlin);

                    uvs[contador] = new Vector2(i % 2, j % 2);
                    colores[contador] = new Color(1, 1, 1);
                    datos[contador++].CargarDatos(posicion, valor);
                }

        _marchingCubeMesh.Datos = datos;
        _marchingCubeMesh.Uvs = uvs;
        _marchingCubeMesh.Colores = colores;

        int cantidadDeindices = ((puntosPorEje.x - 1) * (puntosPorEje.y - 1) * (puntosPorEje.z - 1)) * 8;
        int[] indices = new int[cantidadDeindices];
        for (int i = 0, posicion = 0; i < puntosPorEje.x - 1; i++)
            for (int j = 0; j < puntosPorEje.y - 1; j++)
                for (int k = 0; k < puntosPorEje.z - 1; k++, posicion += 8)
                {
                    indices[posicion] = IndicePorEje(i, j, k, puntosPorEje);
                    indices[posicion + 1] = IndicePorEje(i + 1, j, k, puntosPorEje);
                    indices[posicion + 2] = IndicePorEje(i + 1, j, k + 1, puntosPorEje);
                    indices[posicion + 3] = IndicePorEje(i, j, k + 1, puntosPorEje);
                    indices[posicion + 4] = IndicePorEje(i, j + 1, k, puntosPorEje);
                    indices[posicion + 5] = IndicePorEje(i + 1, j + 1, k, puntosPorEje);
                    indices[posicion + 6] = IndicePorEje(i + 1, j + 1, k + 1, puntosPorEje);
                    indices[posicion + 7] = IndicePorEje(i, j + 1, k + 1, puntosPorEje);
                }

        _marchingCubeMesh.Indices = indices;
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
