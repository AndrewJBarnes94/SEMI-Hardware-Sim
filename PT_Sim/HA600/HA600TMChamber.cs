using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;
using System;
using System.Collections.Generic;

public class HA600TMChamber
{
    private float scale;
    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;
    private int vao, vbo, ebo;

    public HA600TMChamber(float scale)
    {
        this.scale = scale;
        numVertices = 7;
        numIndices = 18;

        float bottomYValue = Formulas.pythagorean(-0.4330126f, -0.4f);

        positions = new float[]
        {
            0.0f * scale, 0.0f * scale,             // Center
            0.0f * scale, 0.5f * scale,             // Top
            0.4330127f * scale, 0.25f * scale,      // Top-right
            0.4330126f * scale, -0.4f * scale,      // Bottom-right
            0.0f * scale, -bottomYValue * scale,    // Bottom
            -0.4330126f * scale, -0.4f * scale,     // Bottom-left
            -0.4330127f * scale, 0.25f * scale      // Top-left
        };

        indices = new uint[]
        {
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 5,
            0, 5, 6,
            0, 6, 1
        };
    }

    ~HA600TMChamber()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
    }

    public void Initialize()
    {
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        if (vao == 0 || vbo == 0 || ebo == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(vao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }

    public List<float> GetPositionMap(string point)
    {
        Dictionary<string, List<float>> positionMap = new Dictionary<string, List<float>>
        {
            { "center", new List<float> { positions[0], positions[1] } },
            { "top", new List<float> { positions[2], positions[3] } },
            { "topRight", new List<float> { positions[4], positions[5] } },
            { "bottomRight", new List<float> { positions[6], positions[7] } },
            { "bottom", new List<float> { positions[8], positions[9] } },
            { "bottomLeft", new List<float> { positions[10], positions[11] } },
            { "topLeft", new List<float> { positions[12], positions[13] } }
        };

        return positionMap[point];
    }
}
