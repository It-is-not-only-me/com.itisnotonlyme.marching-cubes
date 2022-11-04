using System.Collections;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class MinimoLimite
    {
        private const int _cantidadDeEsquinas = 8;
        private static Vector3[] _direcciones = new Vector3[_cantidadDeEsquinas]
        {
            new Vector3(  1,  1,  1),
            new Vector3(  1,  1, -1),
            new Vector3(  1, -1,  1),
            new Vector3(  1, -1, -1),
            new Vector3( -1,  1,  1),
            new Vector3( -1,  1, -1),
            new Vector3( -1, -1,  1),
            new Vector3( -1, -1, -1)
        };

        private Vector3[] _esquinas;
        private float[] _coeficienes;

        public MinimoLimite()
        {
            _esquinas = new Vector3[_cantidadDeEsquinas];
            for (int i = 0; i < _cantidadDeEsquinas; i++)
                _esquinas[i] = Vector3.zero;

            _coeficienes = new float[_cantidadDeEsquinas];
            for (int i = 0; i < _cantidadDeEsquinas; i++)
                _coeficienes[i] = .0f;
        }

        public void AgregarPunto(Vector3 punto)
        {
            for (int i = 0; i < _cantidadDeEsquinas; i++)
            {
                Vector3 direccion = _direcciones[i];
                float coeficienteDireccion = Vector3.Dot(direccion, punto);
                
                if (coeficienteDireccion <= 0)
                    continue;

                float coeficienteEsquina = _coeficienes[i];
                if (coeficienteEsquina < coeficienteDireccion)
                {
                    _direcciones[i] = punto;
                    _coeficienes[i] = coeficienteDireccion;
                }
            }
        }

        public IEnumerable Esquinas()
        {
            foreach (Vector3 esquina in _esquinas)
                yield return esquina;
        }
    }
}
