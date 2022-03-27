using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

[CreateAssetMenu(fileName = "Perlin noise", menuName = "Ejemplo marching cubes/Perlin noise")]
public class ObtenerDatosEjemplo : ObtenerDatosSO
{
    [SerializeField] private uint _dimensionX, _dimensionY, _dimensionZ;
    [SerializeField] private float _velocidad;

    private Vector3Int _dimension;
    private bool _creado = false;
    private float _desfase = 0f;

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

    private void OnEnable()
    {
        _creado = false;
    }

    public override Vector3Int GetDimensiones()
    {
        return Dimension;
    }

    public override IEnumerable<Dato> GetDatos()
    {
        _desfase += _velocidad;
        float noiseScale = 0.9f;
        for (int i = 0; i < _dimensionX; i++)
            for (int j = 0; j < _dimensionY; j++)
                for (int k = 0; k < _dimensionZ; k++)
                {
                    Vector3Int posicion = new Vector3Int(i, j, k);
                    float valor = PerlinNoise3D(i * noiseScale + _desfase, j * noiseScale, k * noiseScale);
                    yield return new Dato(posicion, valor);
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
