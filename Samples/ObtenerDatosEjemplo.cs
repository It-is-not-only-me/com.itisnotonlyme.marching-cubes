using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using ItIsNotOnlyMe.MarchingCubes;

[CreateAssetMenu(fileName = "Perlin noise", menuName = "Ejemplo marching cubes/Perlin noise")]
public class ObtenerDatosEjemplo : ObtenerDatosSO
{
    [SerializeField] private uint _dimensionX, _dimensionY, _dimensionZ;
    [SerializeField] private float _scale;

    [Space]

    [SerializeField] private float _velocidad;
    [SerializeField] private float _noiseScale;

    private Vector3Int _dimension;
    private bool _creado = false;
    private float _desfase = 0f;
    private Dato[] _datos;

    public Vector3Int Dimension
    {
        get
        {
            if (!_creado)
            {
                _dimension = new Vector3Int((int)_dimensionX, (int)_dimensionY, (int)_dimensionZ);
                _creado = true;
            }
            return _dimension;
        }
    }

    public int Cantidad
    {
        get
        {
            return (int)_dimensionX * (int)_dimensionY * (int)_dimensionZ;
        }
    }

    private void OnEnable()
    {
        _creado = false;
        _datos = new Dato[Cantidad];
    }

    public override Vector3Int GetDimensiones()
    {
        return Dimension;
    }

    public override IEnumerable<Dato> GetDatos()
    {
        _desfase += _velocidad;
        float noiseScale = 0.05f;
        for (int i = 0; i < _dimensionX; i++)
            for (int j = 0; j < _dimensionY; j++)
                for (int k = 0; k < _dimensionZ; k++)
                {
                    Vector3 posicion = new Vector3(i * _scale, j * _scale, k * _scale);
                    float valorPerlin = Mathf.PerlinNoise(i * noiseScale + _desfase, k * noiseScale);
                    float valor = valorPerlin * _dimensionY - (float)j;
                    Dato dato = _datos[k + j * _dimensionZ + i * _dimensionZ * _dimensionY];
                    dato.CargarDatos(posicion, valor);
                    yield return dato;
                }
    }

    public static float PerlinNoise3D(float x, float y, float z)
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
