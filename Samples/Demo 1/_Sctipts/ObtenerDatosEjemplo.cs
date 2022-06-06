using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using ItIsNotOnlyMe.MarchingCubes;

[CreateAssetMenu(fileName = "Perlin noise", menuName = "MarchingCubes/Ejemplo/Perlin noise")]
public class ObtenerDatosEjemplo : ObtenerDatosSO
{
    [SerializeField] private uint _dimensionX, _dimensionY, _dimensionZ;
    [SerializeField] private float _scale;
    [SerializeField] private Vector3 _corrimiento;

    [Space]

    [SerializeField] private float _velocidad;
    [SerializeField] private float _noiseScale;
    private float _desfase = 0f;

    public override Vector3Int Dimension
    {
        get => new Vector3Int((int)_dimensionX, (int)_dimensionY, (int)_dimensionZ);
    }

    public override int Id
    {
        get => GetInstanceID();
    }

    public override IEnumerable<Dato> GetDatos()
    {
        Dato dato = new Dato(Vector3.zero, 0);
        _desfase += _velocidad;
        float noiseScale = 0.05f;
        for (int i = 0; i < _dimensionX; i++)
            for (int j = 0; j < _dimensionY; j++)
                for (int k = 0; k < _dimensionZ; k++)
                {
                    Vector3 posicion = new Vector3(i * _scale, j * _scale, k * _scale) + _corrimiento;
                    float valorPerlin = Mathf.PerlinNoise(i * noiseScale + _desfase, k * noiseScale);
                    float valor = valorPerlin * _dimensionY - j;
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
