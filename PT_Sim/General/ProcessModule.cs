using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;

public class ProcessModule
{
    private float _scale;

    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;

    private int vao, vbo, ebo;

    private float posAx, posAy;
    private float posBx, posBy;
    private float posCx, posCy;
    private float posDx, posDy;

    // Wafer Platform
    private (float, float) waferPlatformCenter;
    private float waferPlatformRadius;

    private int waferPlatformNumCircVertices;
    private int waferPlatformNumVertices;
    private int waferPlatformNumIndices;
    private float[] waferPlatformPositions;
    private uint[] waferPlatformIndices;

    private int waferPlatformVao, waferPlatformVbo, waferPlatformEbo;

    // Inner Chamber Half-Circle    
    private int numHalfCircleVertices;
    private int numHalfCircleIndices;
    private float[] halfCirclePositions;
    private uint[] halfCircleIndices;

    private int halfCircleVao, halfCircleVbo, halfCircleEbo;

    // Inner Chamber Rectangle
    private int innerRecNumVertices;
    private int innerRecNumIndices;
    private float[] innerRecPositions;
    private uint[] innerRecIndices;

    private int innerRecVao, innerRecVbo, innerRecEbo;

    // Inner Wafer Platform
    private int innerWaferPlatformNumVertices;
    private float[] innerWaferPlatformPositions;
    private uint[] innerWaferPlatformIndices;

    private int innerWaferPlatformVao, innerWaferPlatformVbo, innerWaferPlatformEbo;

    private const float PI = 3.14159265359323846f;

    public ProcessModule(
        float scale,

        // Outer Chamber
        float posAx, float posAy,
        float posBx, float posBy,
        float posCx, float posCy,
        float posDx, float posDy,

        // Wafer Platform
        (float, float) waferPlatformCenter,
        float waferPlatformRadius,

        // Inner Chamber Half-Circle
        float halfCircleStartX,
        float halfCircleStartY,
        float halfCircleEndX,
        float halfCircleEndY,

        // Inner Chamber Rectangle
        float posEx, float posEy,
        float posFx, float posFy,
        float posGx, float posGy,
        float posHx, float posHy
    )
    {
        this._scale = scale;

        // Outer Chamber
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

        // PM Wafer Platform
        this.waferPlatformCenter = waferPlatformCenter;
        this.waferPlatformRadius = waferPlatformRadius;

        waferPlatformNumCircVertices = 60;
        waferPlatformNumVertices = waferPlatformNumCircVertices + 1; // Center vertex
        waferPlatformNumIndices = waferPlatformNumVertices * 3;

        waferPlatformPositions = new float[waferPlatformNumVertices * 2];
        waferPlatformIndices = new uint[waferPlatformNumIndices];

        // PM Wafer Platform
        int index = 0;

        // Center Vertex
        waferPlatformPositions[index++] = waferPlatformCenter.Item1; // X
        waferPlatformPositions[index++] = waferPlatformCenter.Item2; // Y

        // Circle Vertices
        for (int i = 0; i < waferPlatformNumCircVertices; i++)
        {
            float waferPlatformAngle = (float)(2.0 * System.Math.PI * i / waferPlatformNumCircVertices);
            waferPlatformPositions[index++] = waferPlatformCenter.Item1 + waferPlatformRadius * (float)System.Math.Cos(waferPlatformAngle); // X
            waferPlatformPositions[index++] = waferPlatformCenter.Item2 + waferPlatformRadius * (float)System.Math.Sin(waferPlatformAngle); // Y
        }

        index = 0;

        // Define Indices (Triangles forming a fan shape)
        for (int i = 0; i < waferPlatformNumCircVertices; i++)
        {
            waferPlatformIndices[index++] = 0; // Center vertex
            waferPlatformIndices[index++] = (uint)(i + 1);
            waferPlatformIndices[index++] = (uint)((i + 1) % waferPlatformNumCircVertices + 1); // Loop around
        }

        // Inner Chamber Half-Circle
        float halfCircleCenterX = (halfCircleStartX + halfCircleEndX) / 2.0f;
        float halfCircleCenterY = (halfCircleStartY + halfCircleEndY) / 2.0f;
        float halfCircleRadius = Formulas.distance(halfCircleStartX, halfCircleStartY, halfCircleEndX, halfCircleEndY) / 2.0f;

        // Calculate the angle of the line segment
        float halfCircleAngle = (float)System.Math.Atan2(halfCircleEndY - halfCircleStartY, halfCircleEndX - halfCircleStartX);

        // Increase the number of vertices for a smoother half-circle
        numHalfCircleVertices = 21; // Increased from 11 to 21
        halfCirclePositions = new float[(numHalfCircleVertices + 1) * 2]; // +1 for the center vertex
        halfCircleIndices = new uint[numHalfCircleVertices * 3]; // 3 indices per triangle

        // Center vertex
        halfCirclePositions[0] = halfCircleCenterX * scale;
        halfCirclePositions[1] = halfCircleCenterY * scale;

        // Half-Circle Vertices
        for (int i = 0; i <= numHalfCircleVertices; ++i)
        {
            float theta = halfCircleAngle + -PI * i / numHalfCircleVertices;
            halfCirclePositions[2 * i] = halfCircleCenterX * scale + halfCircleRadius * (float)System.Math.Cos(theta); // x
            halfCirclePositions[2 * i + 1] = halfCircleCenterY * scale + halfCircleRadius * (float)System.Math.Sin(theta); // y
        }

        // Half-Circle Indices
        index = 0;
        for (int i = 1; i <= numHalfCircleVertices; ++i)
        {
            halfCircleIndices[index++] = 0; // Center vertex
            halfCircleIndices[index++] = (uint)i;
            halfCircleIndices[index++] = (uint)(i % numHalfCircleVertices + 1);
        }

        // Inner Chamber Rectangle
        innerRecNumVertices = 4;
        innerRecNumIndices = 6;
        innerRecPositions = new float[]
        {
            posEx * scale, posEy * scale,
            posFx * scale, posFy * scale,
            posGx * scale, posGy * scale,
            posHx * scale, posHy * scale
        };

        // Inner Chamber Rectangle Indices
        innerRecIndices = new uint[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // Inner Wafer Platform
        innerWaferPlatformNumVertices = waferPlatformNumCircVertices;
        innerWaferPlatformPositions = new float[innerWaferPlatformNumVertices * 2];
        innerWaferPlatformIndices = new uint[innerWaferPlatformNumVertices];
        float innerWaferPlatformRadius = 0.9f * waferPlatformRadius; // 80% of the wafer platform radius

        for (int i = 0; i < innerWaferPlatformNumVertices; i++)
        {
            float angle = 2.0f * (float)Math.PI * i / innerWaferPlatformNumVertices;
            innerWaferPlatformPositions[2 * i] = waferPlatformCenter.Item1 + innerWaferPlatformRadius * (float)Math.Cos(angle);
            innerWaferPlatformPositions[2 * i + 1] = waferPlatformCenter.Item2 + innerWaferPlatformRadius * (float)Math.Sin(angle);
            innerWaferPlatformIndices[i] = (uint)i;
        }
    }

    ~ProcessModule()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);

        GL.DeleteVertexArray(waferPlatformVao);
        GL.DeleteBuffer(waferPlatformVbo);
        GL.DeleteBuffer(waferPlatformEbo);

        GL.DeleteVertexArray(halfCircleVao);
        GL.DeleteBuffer(halfCircleVbo);
        GL.DeleteBuffer(halfCircleEbo);

        GL.DeleteVertexArray(innerRecVao);
        GL.DeleteBuffer(innerRecVbo);
        GL.DeleteBuffer(innerRecEbo);

        GL.DeleteVertexArray(innerWaferPlatformVao);
        GL.DeleteBuffer(innerWaferPlatformVbo);
        GL.DeleteBuffer(innerWaferPlatformEbo);
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

        // Wafer Platform
        // Generate OpenGL Buffers
        GL.GenVertexArrays(1, out waferPlatformVao);
        GL.GenBuffers(1, out waferPlatformVbo);
        GL.GenBuffers(1, out waferPlatformEbo);
        if (waferPlatformVao == 0 || waferPlatformVbo == 0 || waferPlatformEbo == 0)
        {
            Logger.Log("Error:", "PM Wafer Platform VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(waferPlatformVao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, waferPlatformVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, waferPlatformPositions.Length * sizeof(float), waferPlatformPositions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, waferPlatformEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, waferPlatformIndices.Length * sizeof(uint), waferPlatformIndices, BufferUsageHint.StaticDraw);

        // Define vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Inner Chamber Half-Circle
        GL.GenVertexArrays(1, out halfCircleVao);
        GL.GenBuffers(1, out halfCircleVbo);
        GL.GenBuffers(1, out halfCircleEbo);

        if (halfCircleVao == 0 || halfCircleVbo == 0 || halfCircleEbo == 0)
        {
            Logger.Log("Error:", "PM Inner Chamber Half-Circle VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(halfCircleVao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, halfCircleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, halfCirclePositions.Length * sizeof(float), halfCirclePositions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, halfCircleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, halfCircleIndices.Length * sizeof(uint), halfCircleIndices, BufferUsageHint.StaticDraw);

        // Define vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Inner Chamber Rectangle
        GL.GenVertexArrays(1, out innerRecVao);
        GL.GenBuffers(1, out innerRecVbo);
        GL.GenBuffers(1, out innerRecEbo);

        if (innerRecVao == 0 || innerRecVbo == 0 || innerRecEbo == 0)
        {
            Logger.Log("Error:", "PM Inner Chamber Rectangle VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(innerRecVao);

        // Upload Vertex Data
        GL.BindBuffer(BufferTarget.ArrayBuffer, innerRecVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, innerRecPositions.Length * sizeof(float), innerRecPositions, BufferUsageHint.StaticDraw);

        // Upload Index Data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, innerRecEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, innerRecIndices.Length * sizeof(uint), innerRecIndices, BufferUsageHint.StaticDraw);

        // Define Vertex Attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Inner Wafer Platform
        GL.GenVertexArrays(1, out innerWaferPlatformVao);
        GL.GenBuffers(1, out innerWaferPlatformVbo);
        GL.GenBuffers(1, out innerWaferPlatformEbo);

        if (innerWaferPlatformVao == 0 || innerWaferPlatformVbo == 0 || innerWaferPlatformEbo == 0)
        {
            Logger.Log("Error:", "Inner Wafer Platform VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(innerWaferPlatformVao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, innerWaferPlatformVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, innerWaferPlatformPositions.Length * sizeof(float), innerWaferPlatformPositions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, innerWaferPlatformEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, innerWaferPlatformIndices.Length * sizeof(uint), innerWaferPlatformIndices, BufferUsageHint.StaticDraw);

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

            // === OUTER SHELL ===
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f); // Black outline
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Gray fill
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // === INNER CHAMBER PERIMETER (Rectangle + Half-Circle) ===
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f); // Black outline

            // Draw rectangle first
            GL.BindVertexArray(innerRecVao);
            GL.DrawElements(PrimitiveType.LineLoop, innerRecNumIndices, DrawElementsType.UnsignedInt, 0);

            // Then draw half-circle on top
            GL.BindVertexArray(halfCircleVao);
            GL.DrawElements(PrimitiveType.LineLoop, halfCircleIndices.Length, DrawElementsType.UnsignedInt, 0);

            // === INNER CHAMBER FILLED INTERIOR ===
            GL.Uniform4(location, 0.6f, 0.6f, 0.6f, 1.0f); // Gray fill

            // Fill rectangle
            GL.BindVertexArray(innerRecVao);
            GL.DrawElements(PrimitiveType.Triangles, innerRecNumIndices, DrawElementsType.UnsignedInt, 0);

            // Fill half-circle
            GL.BindVertexArray(halfCircleVao);
            GL.DrawElements(PrimitiveType.Triangles, halfCircleIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Gray fill

            // === PM WAFER PLATFORM ===
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f); // Black outline
            GL.BindVertexArray(waferPlatformVao);
            GL.DrawElements(PrimitiveType.LineLoop, waferPlatformNumIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Gray fill
            GL.DrawElements(PrimitiveType.Triangles, waferPlatformNumIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // === INNER WAFER PLATFORM ===
            GL.LineWidth(0.1f);
            GL.Uniform4(location, 0.25f, 0.25f, 0.25f, 1.0f); // Black outline
            GL.BindVertexArray(innerWaferPlatformVao);
            GL.DrawElements(PrimitiveType.LineLoop, innerWaferPlatformNumVertices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }
}
