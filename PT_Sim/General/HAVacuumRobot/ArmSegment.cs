using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

public class RobotArmAppendage
{
    private float scale;
    private int numCircleVertices;
    private int numRectangleVertices;
    private int numCircleIndices;
    private int numRectangleIndices;

    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;

    private float[] circlePositions;
    private float[] rectanglePositions;

    private float[] initialCirclePositions;
    private float[] initialRectanglePositions;

    private uint[] circleIndices;
    private uint[] rectangleIndices;

    private const float PI = 3.14159265358979323846f;

    public RobotArmAppendage(float scale)
    {
        this.scale = scale / 3;
        numCircleVertices = 2 * (20 + 1);
        numRectangleVertices = 4;
        numCircleIndices = 2 * 3 * 20;
        numRectangleIndices = 6;

        circlePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];

        initialCirclePositions = new float[numCircleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];

        circleIndices = new uint[numCircleIndices];
        rectangleIndices = new uint[numRectangleIndices];
    }

    ~RobotArmAppendage()
    {
        GL.DeleteVertexArray(circleVao);
        GL.DeleteBuffer(circleVbo);
        GL.DeleteBuffer(circleEbo);
        GL.DeleteVertexArray(rectangleVao);
        GL.DeleteBuffer(rectangleVbo);
        GL.DeleteBuffer(rectangleEbo);
    }

    public void Initialize()
    {
        int index = 0;

        // Left half-circle vertices
        for (int i = 0; i <= 20; i++)
        {
            float theta = -PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (0.5f + 0.22f * (float)Math.Cos(theta)); // Left Half-Circle (was 0.4f)
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.22f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        // Right half-circle vertices
        for (int i = 0; i <= 20; i++)
        {
            float theta = PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (-0.5f + 0.22f * (float)Math.Cos(theta)); // Right Half-Circle (was -0.4f)
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.22f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        float[] rectVerts = {
            0.5f,  0.22f, // Top right
            0.5f, -0.22f, // Bottom right
            -0.5f, -0.22f, // Bottom left
            -0.5f,  0.22f  // Top left
        };

        for (int i = 0; i < rectanglePositions.Length; i++)
        {
            rectanglePositions[i] = scale * rectVerts[i];
            initialRectanglePositions[i] = rectanglePositions[i];
        }

        // Translate to center
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.5f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.5f, 0.0f);
        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.5f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.5f, 0.0f);

        // Indices for circles
        index = 0;
        for (int i = 0; i < 20; i++)
        {
            circleIndices[index++] = 0;
            circleIndices[index++] = (uint)i;
            circleIndices[index++] = (uint)(i + 1);
        }

        for (int i = 21; i < 41; i++)
        {
            circleIndices[index++] = 21;
            circleIndices[index++] = (uint)i;
            circleIndices[index++] = (uint)(i + 1);
        }

        // Indices for rectangle
        rectangleIndices = new uint[] { 0, 1, 2, 0, 2, 3 };

        // OpenGL Buffers Initialization
        CreateBuffer(ref circleVao, ref circleVbo, ref circleEbo, circlePositions, circleIndices);
        CreateBuffer(ref rectangleVao, ref rectangleVbo, ref rectangleEbo, rectanglePositions, rectangleIndices);
    }

    private void CreateBuffer(ref int vao, ref int vbo, ref int ebo, float[] positions, uint[] indices)
    {
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);
    }

    public void UpdateRotation(float radians, float centerX, float centerY)
    {
        float cosA = (float)Math.Cos(-radians);
        float sinA = (float)Math.Sin(-radians);

        ApplyRotation(circlePositions, initialCirclePositions, numCircleVertices, cosA, sinA, centerX, centerY);
        ApplyRotation(rectanglePositions, initialRectanglePositions, numRectangleVertices, cosA, sinA, centerX, centerY);

        UpdateBuffer(circleVbo, circlePositions);
        UpdateBuffer(rectangleVbo, rectanglePositions);
    }

    private void ApplyRotation(float[] positions, float[] initialPositions, int numVertices, float cosA, float sinA, float centerX, float centerY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            float x = initialPositions[i] - centerX;
            float y = initialPositions[i + 1] - centerY;

            positions[i] = cosA * x - sinA * y + centerX;
            positions[i + 1] = sinA * x + cosA * y + centerY;
        }
    }

    private void UpdateBuffer(int vbo, float[] positions)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, positions.Length * sizeof(float), positions);
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            Draw(circleVao, numCircleIndices);
            Draw(rectangleVao, numRectangleIndices);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            Draw(circleVao, numCircleIndices, PrimitiveType.Triangles);
            Draw(rectangleVao, numRectangleIndices, PrimitiveType.Triangles);
        }

        shader.Unbind();
    }

    private void Draw(int vao, int numIndices, PrimitiveType mode = PrimitiveType.LineLoop)
    {
        GL.BindVertexArray(vao);
        GL.DrawElements(mode, numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
        GL.BindVertexArray(0);
    }

    private void TranslateToCenter(float[] positions, int numVertices, float offsetX, float offsetY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            positions[i] += offsetX;
            positions[i + 1] += offsetY;
        }
    }

    public void TranslateArbitrary(float[] positions, int numVertices, float offsetX, float offsetY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            positions[i] += offsetX;
            positions[i + 1] += offsetY;
        }

        // Update the vertex buffer with the new positions
        if (positions == circlePositions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numCircleVertices * 2, circlePositions);
        }
        else if (positions == rectanglePositions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rectangleVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRectangleVertices * 2, rectanglePositions);
        }
    }

    public Tuple<float, float> CalculateRectangleHeightMidpoint(string side)
    {
        // Midpoint of the rectangle's height on the left side
        if (side == "left")
        {
            float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
            float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
            return Tuple.Create(midpointX, midpointY);
        }
        // Midpoint of the rectangle's height on the right side
        else if (side == "right")
        {
            float midpointX = (rectanglePositions[4] + rectanglePositions[6]) / 2;
            float midpointY = (rectanglePositions[5] + rectanglePositions[7]) / 2;
            return Tuple.Create(midpointX, midpointY);
        }
        // Default return value for unexpected side values
        return Tuple.Create(0.0f, 0.0f);
    }

    public Tuple<float, float> CalculateRedDotPosition(string side)
    {
        // Calculate the red dot position based on the rectangle's height midpoint
        return CalculateRectangleHeightMidpoint(side);
    }

    public void TranslateToPosition(float x, float y)
    {
        // Calculate the offset needed to translate the appendage to the specified position
        var redDotPosition = CalculateRedDotPosition("right");
        float offsetX = x - redDotPosition.Item1;
        float offsetY = y - redDotPosition.Item2;

        // Translate the entire geometry
        TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
        TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
    }
}
