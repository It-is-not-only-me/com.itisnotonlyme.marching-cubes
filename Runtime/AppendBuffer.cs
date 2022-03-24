using UnityEngine;
using UnityEngine.Rendering;

public class AppendBuffer : MonoBehaviour
{
    public Material _material;
    public ComputeShader _shader;
    public int _size = 8;

    ComputeBuffer _buffer, _argBuffer;

    private void Awake()
    {
        _buffer = new ComputeBuffer(32 * 32 * 32, sizeof(float) * 3, ComputeBufferType.Append);
        _argBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

        _shader.SetBuffer(0, "buffer", _buffer);
        _shader.SetFloat("size", _size);
        _shader.Dispatch(0, _size / 8, _size / 8, _size / 8);

        int[] args = new int[] { 0, 1, 0, 0 };
        _argBuffer.SetData(args);
        ComputeBuffer.CopyCount(_buffer, _argBuffer, 0);
        _argBuffer.GetData(args);
    }

    private void OnPostRender()
    {
        _material.SetPass(0);
        _material.SetBuffer("buffer", _buffer);
        Graphics.DrawProceduralIndirectNow(MeshTopology.Points, _argBuffer, 0);
    }

    private void OnDestroy()
    {
        _argBuffer.Release();
        _buffer.Release();
    }
}