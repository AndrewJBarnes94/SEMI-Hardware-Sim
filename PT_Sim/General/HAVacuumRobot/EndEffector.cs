using System;
using OpenTK.Graphics.OpenGL;

public class EndEffector
{
    private float scale;

    private int numCircleVertices;
    private int numCircleIndices;
    private int circleVao, circleVbo, circleEbo;
    private float[] circlePositions;
    private float[] initialCirclePositions;
    private uint[] circleIndices;

    private int numRectangleVertices;
    private int numRectangleIndices;
    private int rectangleVao, rectangleVbo, rectangleEbo;
    private float[] rectanglePositions;
    private float[] initialRectanglePositions;
    private uint[] rectangleIndices;

    private int numScribePlateVertices;
    private int numscribePlateIndices;
    private int scribePlateVao, scribePlateVbo, scribePlatebo;
    private float[] scribePlatePositions;
    private float[] initialScribePlatePositions;
    private uint[] scribePlateIndices;


    private int numFork1Vertices;
    private int numFork1Indices;
    private int fork1Vao, fork1Vbo, fork1Ebo;
    private float[] fork1Positions;
    private float[] initialFork1Positions;
    private uint[] fork1Indices;

    private int numFork2Vertices;
    private int numFork2Indices;
    private int fork2Vao, fork2Vbo, fork2Ebo;
    private float[] fork2Positions;
    private float[] initialFork2Positions;
    private uint[] fork2Indices;

    private const float PI = 3.14159265358979323846f;

    public EndEffector(float scale)
    {
        this.scale = scale/3;

        numCircleVertices = 2 * (20 + 1);
        numCircleIndices = 2 * 3 * 20;
        circlePositions = new float[numCircleVertices * 2];
        initialCirclePositions = new float[numCircleVertices * 2];
        circleIndices = new uint[numCircleIndices];

        numRectangleVertices = 4;
        numRectangleIndices = 6;
        rectanglePositions = new float[numRectangleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];
        rectangleIndices = new uint[numRectangleIndices];

        numFork1Vertices = 4;
        numFork1Indices = 6;
        fork1Positions = new float[numFork1Vertices * 2];
        initialFork1Positions = new float[numFork1Vertices * 2];
        fork1Indices = new uint[numFork1Indices];

        numFork2Vertices = 4;
        numFork2Indices = 6;
        fork2Positions = new float[numFork2Vertices * 2];
        initialFork2Positions = new float[numFork2Vertices * 2];
        fork2Indices = new uint[numFork2Indices];

        numScribePlateVertices = 42; // 21 points for each parabola
        numscribePlateIndices = 6 * 20 + 2 * 21;
        scribePlatePositions = new float[numScribePlateVertices * 2];
        initialScribePlatePositions = new float[numScribePlateVertices * 2];
        scribePlateIndices = new uint[numscribePlateIndices];
    }

    ~EndEffector()
    {
        GL.DeleteVertexArray(circleVao);
        GL.DeleteBuffer(circleVbo);
        GL.DeleteBuffer(circleEbo);

        GL.DeleteVertexArray(rectangleVao);
        GL.DeleteBuffer(rectangleVbo);
        GL.DeleteBuffer(rectangleEbo);

        GL.DeleteVertexArray(fork1Vao);
        GL.DeleteBuffer(fork1Vbo);
        GL.DeleteBuffer(fork1Ebo);

        GL.DeleteVertexArray(fork2Vao);
        GL.DeleteBuffer(fork2Vbo);
        GL.DeleteBuffer(fork2Ebo);

        GL.DeleteVertexArray(scribePlateVao);
        GL.DeleteBuffer(scribePlateVbo);
        GL.DeleteBuffer(scribePlatebo);
    }

    public void Initialize()
    {
        int index = 0;

        // Left half-circle vertices
        for (int i = 0; i <= 20; i++)
        {
            float theta = -PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (0.5f + 0.2f * (float)Math.Cos(theta)); // Left Half-Circle (was 0.4f)
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        float[] rectVerts = {
            -0.9f,  0.2f, // Top right
            -0.9f, -0.2f, // Bottom right
            0.5f, -0.2f, // Bottom left
            0.5f,  0.2f  // Top left
        };

        for (int i = 0; i < rectanglePositions.Length; i++)
        {
            rectanglePositions[i] = scale * rectVerts[i];
            initialRectanglePositions[i] = rectanglePositions[i];
        }

        float[] fork1Verts =
        {
            -1.2f,  0.25f, // Top right
            -1.2f, 0.20f, // Bottom right
            -0.75f, 0.20f, // Bottom left
            -0.75f,  0.25f  // Top left
        };

        for (int i = 0; i < fork1Positions.Length; i++)
        {
            fork1Positions[i] = scale * fork1Verts[i];
            initialFork1Positions[i] = fork1Positions[i];
        }

        float[] fork2Verts =
        {
            -1.2f, -0.25f, // Top right
            -1.2f, -0.20f, // Bottom right
            -0.75f, -0.20f, // Bottom left
            -0.75f,  -0.25f  // Top left
        };

        for (int i = 0; i < fork2Positions.Length; i++)
        {
            fork2Positions[i] = scale * fork2Verts[i];
            initialFork2Positions[i] = fork2Positions[i];
        }

        // Scribe plate vertices
        InitializeScribePlateVertices(0.1f, 0.0f, 0.0f, -1.0f, 1.0f, 21); // Adjusted 'a' value to 0.1

        // Translate to center
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.55f, 0.0f);
        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.55f, 0.0f);

        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.55f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.55f, 0.0f);

        TranslateToCenter(fork1Positions, numFork1Vertices, -scale * 0.65f, 0.0f);
        TranslateToCenter(initialFork1Positions, numFork1Vertices, -scale * 0.65f, 0.0f);

        TranslateToCenter(fork2Positions, numFork2Vertices, -scale * 0.65f, 0.0f);
        TranslateToCenter(initialFork2Positions, numFork2Vertices, -scale * 0.65f, 0.0f);

        TranslateToCenter(scribePlatePositions, numScribePlateVertices, -scale * 1.5f, 0.0f); // Adjusted offset to move scribePlate to the left
        TranslateToCenter(initialScribePlatePositions, numScribePlateVertices, -scale * 1.5f, 0.0f); // Adjusted offset to move scribePlate to the left

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

        // Indices for fork1
        fork1Indices = new uint[]{ 0, 1, 2, 0, 2, 3 };

        // Indices for fork2
        fork2Indices = new uint[] { 0, 1, 2, 0, 2, 3 };

        // Indices for scribe plate
        InitializeScribePlateIndices();

        // OpenGL Buffers Initialization
        CreateBuffer(ref circleVao, ref circleVbo, ref circleEbo, circlePositions, circleIndices);
        CreateBuffer(ref rectangleVao, ref rectangleVbo, ref rectangleEbo, rectanglePositions, rectangleIndices);
        CreateBuffer(ref fork1Vao, ref fork1Vbo, ref fork1Ebo, fork1Positions, fork1Indices);
        CreateBuffer(ref fork2Vao, ref fork2Vbo, ref fork2Ebo, fork2Positions, fork2Indices);
        CreateBuffer(ref scribePlateVao, ref scribePlateVbo, ref scribePlatebo, scribePlatePositions, scribePlateIndices);

    }

    private void InitializeScribePlateVertices(float a, float b, float c, float startX, float endX, int numPoints)
    {
        int index = 0;
        float step = (endX - startX) / (numPoints - 1);
        float scribeScaleX = 0.25f; // Slightly increased scaling factor to make the scribe plate wider in the x direction
        float scribeScaleY = 1.0f; // Scaling factor to increase the distance between the parabolas
        for (int i = 0; i < numPoints; ++i)
        {
            float x = startX + i * step;
            float y = a * x * x + b * x + c;

            // First parabola
            scribePlatePositions[index] = -y * scale * scribeScaleY; // Negate y to rotate 180 degrees
            initialScribePlatePositions[index] = scribePlatePositions[index];
            index++;
            scribePlatePositions[index] = -x * scale * scribeScaleX; // Negate x to rotate 180 degrees
            initialScribePlatePositions[index] = scribePlatePositions[index];
            index++;

            // Second parabola (parallel and shifted)
            scribePlatePositions[index] = -(y - 0.2f) * scale * scribeScaleY; // Negate y to rotate 180 degrees
            initialScribePlatePositions[index] = scribePlatePositions[index];
            index++;
            scribePlatePositions[index] = -x * scale * scribeScaleX; // Negate x to rotate 180 degrees
            initialScribePlatePositions[index] = scribePlatePositions[index];
            index++;
        }
    }

    private void InitializeScribePlateIndices()
    {
        int index = 0;
        int numPoints = numScribePlateVertices / 2;

        // Triangles to fill the area between the parabolas
        for (int i = 0; i < numPoints - 1; ++i)
        {
            uint topLeft = (uint)(i * 2);
            uint bottomLeft = (uint)(i * 2 + 1);
            uint topRight = (uint)(i * 2 + 2);
            uint bottomRight = (uint)(i * 2 + 3);

            // Triangle 1
            scribePlateIndices[index++] = topLeft;
            scribePlateIndices[index++] = bottomLeft;
            scribePlateIndices[index++] = topRight;

            // Triangle 2
            scribePlateIndices[index++] = topRight;
            scribePlateIndices[index++] = bottomLeft;
            scribePlateIndices[index++] = bottomRight;
        }

        // LineStrip indices for top parabola
        for (int i = 0; i < numPoints; ++i)
        {
            scribePlateIndices[index++] = (uint)(i * 2);
        }

        // LineStrip indices for bottom parabola
        for (int i = 0; i < numPoints; ++i)
        {
            scribePlateIndices[index++] = (uint)(i * 2 + 1);
        }
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
        ApplyRotation(fork1Positions, initialFork1Positions, numFork1Vertices, cosA, sinA, centerX, centerY);
        ApplyRotation(fork2Positions, initialFork2Positions, numFork2Vertices, cosA, sinA, centerX, centerY);
        ApplyRotation(scribePlatePositions, initialScribePlatePositions, numScribePlateVertices, cosA, sinA, centerX, centerY);


        UpdateBuffer(circleVbo, circlePositions);
        UpdateBuffer(rectangleVbo, rectanglePositions);
        UpdateBuffer(fork1Vbo, fork1Positions);
        UpdateBuffer(fork2Vbo, fork2Positions);
        UpdateBuffer(scribePlateVbo, scribePlatePositions);

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

            // Set color for normal outlines (black)
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            // Draw black outlines for circle, rectangle, forks
            Draw(circleVao, numCircleIndices);
            Draw(rectangleVao, numRectangleIndices);
            Draw(fork1Vao, numFork1Indices);
            Draw(fork2Vao, numFork2Indices);

            GL.BindVertexArray(scribePlateVao);

            int numPoints = numScribePlateVertices / 2;
            int triangleIndexCount = (numPoints - 1) * 6; // each quad made of 2 triangles

            // Top parabola LineStrip
            GL.DrawElements(PrimitiveType.LineStrip, numPoints, DrawElementsType.UnsignedInt, (IntPtr)(triangleIndexCount * sizeof(uint)));

            // Bottom parabola LineStrip
            GL.DrawElements(PrimitiveType.LineStrip, numPoints, DrawElementsType.UnsignedInt, (IntPtr)((triangleIndexCount + numPoints) * sizeof(uint)));

            GL.BindVertexArray(0);


            // Switch to fill color
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            // Fill other shapes
            Draw(circleVao, numCircleIndices, PrimitiveType.Triangles);
            Draw(rectangleVao, numRectangleIndices, PrimitiveType.Triangles);
            Draw(fork1Vao, numFork1Indices, PrimitiveType.Triangles);
            Draw(fork2Vao, numFork2Indices, PrimitiveType.Triangles);

            GL.BindVertexArray(scribePlateVao);
            GL.DrawElements(PrimitiveType.Triangles, (numPoints - 1) * 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.BindVertexArray(0);
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
        else if (positions == fork1Positions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, fork1Vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numFork1Vertices * 2, fork1Positions);
        }
        else if (positions == fork2Positions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, fork2Vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numFork2Vertices * 2, fork2Positions);
        }
        else if (positions == scribePlatePositions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, scribePlateVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numScribePlateVertices * 2, scribePlatePositions);
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
        TranslateArbitrary(fork1Positions, numFork1Vertices, offsetX, offsetY);
        TranslateArbitrary(fork2Positions, numFork2Vertices, offsetX, offsetY);
        TranslateArbitrary(scribePlatePositions, numScribePlateVertices, offsetX, offsetY);
    }
}