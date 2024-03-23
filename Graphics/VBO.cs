using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Toryngine.Graphics;

internal class VBO
{
    public int ID;
    public VBO(List<Vector3> data) {
        ID = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector3.SizeInBytes, data.ToArray(), BufferUsageHint.StreamDraw);
    }
    public VBO(List<Vector2> data) {
        ID = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
        GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector2.SizeInBytes, data.ToArray(), BufferUsageHint.StreamDraw);
    }

    public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
    public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    public void Delete() => GL.DeleteBuffer(ID);
}