using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using PT_Sim;

class HAVacuumRobot
{
    private float scale;

    // Base Circle (Original Geometry)
    private int baseNumVertices;
    private int baseNumIndices;
    private float[] basePositions;
    private uint[] baseIndices;
    private int baseVao, baseVbo, baseEbo;

    // Segment1 (Half-Circle)
    private int segment1NumVertices;
    private int segment1NumIndices;
    private float[] segment1Positions;
    private uint[] segment1Indices;
    private int segment1Vao, segment1Vbo, segment1Ebo;

    public HAVacuumRobot(float scale)
    {
        this.scale = scale;
        int baseNumOuterVertices = 60; // Smoothness for full circle
        int segment1NumOuterVertices = 30; // Smoothness for half-circle

        // Base Circle (Original)
        baseNumVertices = baseNumOuterVertices + 1; // Center + outer points
        baseNumIndices = baseNumOuterVertices * 3;
        float baseRadius = 0.25f * scale;
        basePositions = GenerateCircleVertices(baseNumVertices, baseRadius);
        baseIndices = GenerateCircleIndices(baseNumOuterVertices);

        // Segment1 Half-Circle
        segment1NumVertices = segment1NumOuterVertices + 1; // Center + half-circle points
        segment1NumIndices = segment1NumOuterVertices * 3;
        float segment1Radius = 0.15f * scale;
        segment1Positions = GenerateHalfCircleVertices(segment1NumVertices, segment1Radius, 0.3f); // Offset to the right
        segment1Indices = GenerateHalfCircleIndices(segment1NumOuterVertices);
    }

    ~HAVacuumRobot()
    {
        GL.DeleteVertexArray(baseVao);
        GL.DeleteBuffer(baseVbo);
        GL.DeleteBuffer(baseEbo);

        GL.DeleteVertexArray(segment1Vao);
        GL.DeleteBuffer(segment1Vbo);
        GL.DeleteBuffer(segment1Ebo);
    }

    public void Initialize()
    {
        // Base Circle
        baseVao = CreateVAO(basePositions, baseIndices, out baseVbo, out baseEbo);

        // Segment1 (Half-Circle)
        segment1Vao = CreateVAO(segment1Positions, segment1Indices, out segment1Vbo, out segment1Ebo);
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location == 0)
        {
            GL.LineWidth(2.0f);

            // Render Base Circle
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(baseVao);
            GL.DrawElements(PrimitiveType.Triangles, baseNumIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Render Segment1 (Half-Circle)
            GL.Uniform4(location, 1.0f, 0.0f, 0.0f, 1.0f); // Red color
            GL.BindVertexArray(segment1Vao);
            GL.DrawElements(PrimitiveType.Triangles, segment1NumIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }

    private float[] GenerateCircleVertices(int numVertices, float radius)
    {
        float[] positions = new float[numVertices * 2];

        // Center of Circle
        positions[0] = 0.0f;
        positions[1] = 0.0f;

        // Generate outer circle points (360°)
        for (int i = 0; i < numVertices - 1; i++)
        {
            float angle = (float)(i * (2.0 * Math.PI / (numVertices - 1)));
            positions[(i + 1) * 2] = radius * (float)Math.Cos(angle);
            positions[(i + 1) * 2 + 1] = radius * (float)Math.Sin(angle);
        }
        return positions;
    }

    private uint[] GenerateCircleIndices(int numOuterVertices)
    {
        uint[] indices = new uint[numOuterVertices * 3];

        for (int i = 0; i < numOuterVertices; i++)
        {
            indices[i * 3] = 0;
            indices[i * 3 + 1] = (uint)(i + 1);
            indices[i * 3 + 2] = (uint)((i + 1) % numOuterVertices + 1);
        }
        return indices;
    }

    private float[] GenerateHalfCircleVertices(int numVertices, float radius, float xOffset)
    {
        float[] positions = new float[numVertices * 2];

        // Center of Half-Circle
        positions[0] = xOffset;
        positions[1] = 0.0f;

        // Generate outer half-circle points (180° arc)
        for (int i = 0; i < numVertices - 1; i++)
        {
            float angle = (float)(Math.PI * i / (numVertices - 2)); // Only 180° range
            positions[(i + 1) * 2] = xOffset + radius * (float)Math.Cos(angle);
            positions[(i + 1) * 2 + 1] = radius * (float)Math.Sin(angle);
        }
        return positions;
    }

    private uint[] GenerateHalfCircleIndices(int numHalfCircleVertices)
    {
        uint[] indices = new uint[numHalfCircleVertices * 3];

        for (int i = 0; i < numHalfCircleVertices; i++)
        {
            indices[i * 3] = 0;  // Center vertex
            indices[i * 3 + 1] = (uint)(i + 1);
            indices[i * 3 + 2] = (uint)((i + 1) % numHalfCircleVertices + 1);
        }
        return indices;
    }

    private int CreateVAO(float[] positions, uint[] indices, out int vbo, out int ebo)
    {
        int vao;
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);

        return vao;
    }
}
