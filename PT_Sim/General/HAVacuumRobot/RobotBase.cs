using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

public class RobotBase
{
    private int vao, vbo, ebo;
    private float scale;
    private float[] positions;
    private int[] indices;
    private int numVertices = 41; // 40 segments + center
    private int numIndices = 120; // 40 triangles * 3 indices

    public RobotBase(float scale)
    {
        this.scale = scale;
        positions = new float[numVertices * 2];
        indices = new int[numIndices];
    }

    public void Dispose()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
    }

    private void GenerateCircleVertices()
    {
        int index = 0;

        // Center vertex
        positions[index++] = 0.0f;
        positions[index++] = 0.0f;

        // Circle vertices
        float radius = 0.8f * scale;
        for (int i = 0; i <= 40; i++)
        {
            float theta = (float)(2.0 * Math.PI * i / 40); // Convert to float
            positions[index++] = radius * (float)Math.Cos(theta);
            positions[index++] = radius * (float)Math.Sin(theta);
        }

        // Circle indices
        index = 0;
        for (int i = 1; i <= 40; i++)
        {
            indices[index++] = 0;
            indices[index++] = i;
            indices[index++] = (i + 1) % 41; // Ensure the last one wraps around
        }
    }

    public void Initialize()
    {
        GenerateCircleVertices();

        // Generate VAO, VBO, and EBO
        vao = GL.GenVertexArray();
        vbo = GL.GenBuffer();
        ebo = GL.GenBuffer();

        GL.BindVertexArray(vao);

        // Buffer vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

        // Buffer index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

        // Vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    public void Render(Shader shader)
    {
        shader.Bind(); // Bind the shader

        int location = shader.GetUniformLocation("u_Color");
        if (location != -1)
        {
            // Draw the fill
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Metallic gray
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the outline
            GL.LineWidth(4.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f); // Black outline
            GL.DrawArrays(PrimitiveType.LineLoop, 1, 40);
        }
        else
        {
            Console.WriteLine("Uniform location for 'u_Color' is invalid.");
        }

        GL.BindVertexArray(0);
        shader.Unbind();
    }
}

