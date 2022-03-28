using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItIsNotOnlyMe.MarchingCubes
{
    public class AgregarDatosSO : ScriptableObject
    {
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private float _isoLevel;

        private ComputeBuffer _triangulosBuffer, _datosBuffer;
    }
}
