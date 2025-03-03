using System;
using OpenTK.Graphics.OpenGL;

class Base
{
    private float scale;
    private float radius;

    private int numOuterVertices;
    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;
    private int vao, vbo, ebo;

    public Base(float scale)
    {
        this.scale = scale;
        this.radius = 0.2f * scale; // Fix: No re-declaration

        numOuterVertices = 60;
        numVertices = numOuterVertices + 1; // Center vertex
        numIndices = numOuterVertices * 3;  // Triangle count

        positions = new float[numVertices * 2]; // Fix: Correct size
        indices = new uint[numIndices];
    }

    ~Base()
    {
        if (vao != 0) GL.DeleteVertexArray(vao);
        if (vbo != 0) GL.DeleteBuffer(vbo);
        if (ebo != 0) GL.DeleteBuffer(ebo);
    }

    public void Initialize()
    {
        int index = 0;

        // Center Vertex
        positions[index++] = 0.0f; // X
        positions[index++] = 0.0f; // Y

        // Outer Circle Vertices
        for (int i = 0; i < numOuterVertices; i++)
        {
            float angle = (float)(2.0 * Math.PI * i / numOuterVertices);
            positions[index++] = radius * (float)Math.Cos(angle); // X
            positions[index++] = radius * (float)Math.Sin(angle); // Y
        }

        index = 0; // Fix: Reuse index correctly

        // Define indices (Triangles forming a fan shape)
        for (int i = 0; i < numOuterVertices; i++)
        {
            indices[index++] = 0;  // Center vertex
            indices[index++] = (uint)(i + 1);
            indices[index++] = (uint)((i + 1) % numOuterVertices + 1); // Loop around
        }

        // Generate OpenGL Buffers
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        GL.BindVertexArray(vao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Define vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);

            // Draw Outline using Line Loop (No extra indices needed!)
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw Filled Interior (Gray)
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }
}
