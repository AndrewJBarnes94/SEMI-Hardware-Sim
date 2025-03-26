using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;
using System;
using System.Collections.Generic;

public class HA600TMChamber : IDisposable
{
    private float scale;
    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;
    private int vao, vbo, ebo;

    private int circleVao, circleVbo, circleEbo;
    private int numCircleVertices = 100; // Number of vertices for the circle
    private float[] circlePositions;
    private uint[] circleIndices;

    private bool disposed = false;

    public HA600TMChamber(float scale)
    {
        this.scale = scale;
        numVertices = 7;
        numIndices = 18;

        float bottomYValue = Formulas.pythagorean(-0.4330126f, -0.47f);

        positions = new float[]
        {
            0.0f * scale, 0.0f * scale,             // Center
            0.0f * scale, 0.5f * scale,             // Top
            0.4330127f * scale, 0.25f * scale,      // Top-right
            0.4330126f * scale, -0.47f * scale,      // Bottom-right
            0.0f * scale, -bottomYValue * scale,    // Bottom
            -0.4330126f * scale, -0.47f * scale,     // Bottom-left
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

        // Initialize circle positions and indices
        circlePositions = new float[numCircleVertices * 2];
        circleIndices = new uint[numCircleVertices];
        float radius = 0.4f * scale; // Adjust the radius as needed
        for (int i = 0; i < numCircleVertices; i++)
        {
            float angle = 2.0f * (float)Math.PI * i / numCircleVertices;
            circlePositions[2 * i] = radius * (float)Math.Cos(angle);
            circlePositions[2 * i + 1] = radius * (float)Math.Sin(angle);
            circleIndices[i] = (uint)i;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Free managed resources if any
            }

            // Free unmanaged resources
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);

            GL.DeleteVertexArray(circleVao);
            GL.DeleteBuffer(circleVbo);
            GL.DeleteBuffer(circleEbo);

            disposed = true;
        }
    }

    ~HA600TMChamber()
    {
        Dispose(false);
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

        // Initialize circle VAO, VBO, and EBO
        GL.GenVertexArrays(1, out circleVao);
        GL.GenBuffers(1, out circleVbo);
        GL.GenBuffers(1, out circleEbo);

        GL.BindVertexArray(circleVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, circlePositions.Length * sizeof(float), circlePositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, circleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, circleIndices.Length * sizeof(uint), circleIndices, BufferUsageHint.StaticDraw);

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
            // Draw the chamber outline
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the chamber fill
            GL.Uniform4(location, 0.6f, 0.6f, 0.6f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            // Draw the circle outline
            GL.LineWidth(0.75f); // Ensure the line width is set before drawing the circle
            GL.Uniform4(location, 0.5f, 0.5f, 0.5f, 0.5f);
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numCircleVertices, DrawElementsType.UnsignedInt, 0);
            
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
