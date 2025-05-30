﻿using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;

public class CLP
{
    private float scale;

    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;

    private int vao, vbo, ebo;

    private float posAx, posAy;
    private float posBx, posBy;
    private float posCx, posCy;
    private float posDx, posDy;

    // Additional squares
    private float[] positions90;
    private float[] positions80;
    private float[] positions70;
    private int vao90, vbo90, ebo90;
    private int vao80, vbo80, ebo80;
    private int vao70, vbo70, ebo70;

    public CLP(float scale, float posAx, float posAy, float posBx, float posBy, float posCx, float posCy, float posDx, float posDy)
    {
        this.scale = scale;

        this.posAx = posAx;
        this.posAy = posAy;
        this.posBx = posBx;
        this.posBy = posBy;
        this.posCx = posCx;
        this.posCy = posCy;
        this.posDx = posDx;
        this.posDy = posDy;

        numVertices = 4;
        numIndices = 6;
        positions = new float[]
        {
            posAx * scale, posAy * scale,
            posBx * scale, posBy * scale,
            posCx * scale, posCy * scale,
            posDx * scale, posDy * scale
        };
        indices = new uint[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // Calculate the center of the original square
        float centerX = (posAx + posBx + posCx + posDx) / 4.0f * scale;
        float centerY = (posAy + posBy + posCy + posDy) / 4.0f * scale;

        // Calculate positions for the 90% square
        (float, float) newSquare90A = (posAx, posAy);
        (float, float) newSquare90B = (posBx, posBy);
        newSquare90A = Formulas.MovePointTowards(newSquare90A.Item1, newSquare90A.Item2, newSquare90B.Item1, newSquare90B.Item2, 0.02f * scale);
        newSquare90B = Formulas.MovePointTowards(newSquare90B.Item1, newSquare90B.Item2, newSquare90A.Item1, newSquare90A.Item2, 0.02f * scale);

        positions90 = new float[]
        {
            newSquare90A.Item1, newSquare90A.Item2,
            newSquare90B.Item1, newSquare90B.Item2,
            centerX + (posCx * scale - centerX) * 0.9f, centerY + (posCy * scale - centerY) * 0.9f,
            centerX + (posDx * scale - centerX) * 0.9f, centerY + (posDy * scale - centerY) * 0.9f
        };

        // Calculate positions for the 80% square
        positions80 = new float[]
        {
            centerX + (posAx * scale - centerX) * 0.75f, centerY + (posAy * scale - centerY) * 0.75f,
            centerX + (posBx * scale - centerX) * 0.75f, centerY + (posBy * scale - centerY) * 0.75f,
            centerX + (posCx * scale - centerX) * 0.75f, centerY + (posCy * scale - centerY) * 0.75f,
            centerX + (posDx * scale - centerX) * 0.75f, centerY + (posDy * scale - centerY) * 0.75f
        };

        // Calculate positions for the 70% square
        positions70 = new float[]
        {
            centerX + (posAx * scale - centerX) * 0.65f, centerY + (posAy * scale - centerY) * 0.65f,
            centerX + (posBx * scale - centerX) * 0.65f, centerY + (posBy * scale - centerY) * 0.65f,
            centerX + (posCx * scale - centerX) * 0.65f, centerY + (posCy * scale - centerY) * 0.65f,
            centerX + (posDx * scale - centerX) * 0.65f, centerY + (posDy * scale - centerY) * 0.65f
        };
    }

    ~CLP()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);

        GL.DeleteVertexArray(vao90);
        GL.DeleteBuffer(vbo90);
        GL.DeleteBuffer(ebo90);

        GL.DeleteVertexArray(vao80);
        GL.DeleteBuffer(vbo80);
        GL.DeleteBuffer(ebo80);

        GL.DeleteVertexArray(vao70);
        GL.DeleteBuffer(vbo70);
        GL.DeleteBuffer(ebo70);
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

        // Setup vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Setup VAO for 90% square
        GL.GenVertexArrays(1, out vao90);
        GL.GenBuffers(1, out vbo90);
        GL.GenBuffers(1, out ebo90);
        if (vao90 == 0 || vbo90 == 0 || ebo90 == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO for 90% square not initialized correctly");
            return;
        }

        GL.BindVertexArray(vao90);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo90);
        GL.BufferData(BufferTarget.ArrayBuffer, positions90.Length * sizeof(float), positions90, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo90);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Setup VAO for 80% square
        GL.GenVertexArrays(1, out vao80);
        GL.GenBuffers(1, out vbo80);
        GL.GenBuffers(1, out ebo80);
        if (vao80 == 0 || vbo80 == 0 || ebo80 == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO for 80% square not initialized correctly");
            return;
        }

        GL.BindVertexArray(vao80);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo80);
        GL.BufferData(BufferTarget.ArrayBuffer, positions80.Length * sizeof(float), positions80, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo80);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Setup VAO for 70% square
        GL.GenVertexArrays(1, out vao70);
        GL.GenBuffers(1, out vbo70);
        GL.GenBuffers(1, out ebo70);
        if (vao70 == 0 || vbo70 == 0 || ebo70 == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO for 70% square not initialized correctly");
            return;
        }

        GL.BindVertexArray(vao70);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo70);
        GL.BufferData(BufferTarget.ArrayBuffer, positions70.Length * sizeof(float), positions70, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo70);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

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

            // Draw 90% square
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao90);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.6f, 0.6f, 0.6f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw 80% square
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao80);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw 70% square
            GL.LineWidth(0.5f);
            GL.Uniform4(location, 0.25f, 0.25f, 0.25f, 1.0f);
            GL.BindVertexArray(vao70);
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
            { "A", new List<float> { posAx, posAy } },
            { "B", new List<float> { posBx, posBy } },
            { "C", new List<float> { posCx, posCy } },
            { "D", new List<float> { posDx, posDy } },
            { "A90", new List<float> { positions90[0], positions90[1] } },
            { "B90", new List<float> { positions90[2], positions90[3] } },
            { "C90", new List<float> { positions90[4], positions90[5] } },
            { "D90", new List<float> { positions90[6], positions90[7] } },
            { "A80", new List<float> { positions80[0], positions80[1] } },
            { "B80", new List<float> { positions80[2], positions80[3] } },
            { "C80", new List<float> { positions80[4], positions80[5] } },
            { "D80", new List<float> { positions80[6], positions80[7] } },
            { "A70", new List<float> { positions70[0], positions70[1] } },
            { "B70", new List<float> { positions70[2], positions70[3] } },
            { "C70", new List<float> { positions70[4], positions70[5] } },
            { "D70", new List<float> { positions70[6], positions70[7] } }
        };

        return positionMap[point];
    }
}


