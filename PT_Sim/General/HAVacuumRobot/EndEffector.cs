using System;
using OpenTK.Graphics.OpenGL;

public class RobotArmEndEffector
{
    private float scale;

    private int numCircleVertices, numRectangleVertices, numScribePlateVertices;
    private int numCircleIndices, numRectangleIndices, numScribePlateIndices;

    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;
    private int scribePlateVao, scribePlateVbo, scribePlateEbo;

    private float[] circlePositions, rectanglePositions, scribePlatePositions;
    private float[] initialCirclePositions, initialRectanglePositions, initialScribePlatePositions;

    private uint[] circleIndices, rectangleIndices, scribePlateIndices;


    private int numFork1Vertices;
    private int numFork1Indices;
    private int fork1Vao, fork1Vbo, fork1Ebo;
    private float[] fork1Positions;
    private float[] initialFork1Positions;
    private uint[] fork1Indices;

    private int numfork2Vertices;
    private int numfork2Indices;
    private int fork2Vao, fork2Vbo, fork2Ebo;
    private float[] fork2Positions;
    private float[] initialFork2Positions;
    private uint[] fork2Indices;


    private const float PI = 3.14159265358979323846f;

    public RobotArmEndEffector(float scale)
    {
        this.scale = scale / 3;

        numCircleVertices = 21;
        numRectangleVertices = 4;
        numScribePlateVertices = 42; // 21 points for each parabola

        numCircleIndices = 3 * 20;
        numRectangleIndices = 6;
        numScribePlateIndices = 6 * 20 + 2 * 21; // 2 triangles per segment, 20 segments + 2 * 21 for the outline

        // Initialize arrays to prevent `NullReferenceException`
        circlePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];
        scribePlatePositions = new float[numScribePlateVertices * 2];

        initialCirclePositions = new float[numCircleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];
        initialScribePlatePositions = new float[numScribePlateVertices * 2];

        circleIndices = new uint[numCircleIndices];
        rectangleIndices = new uint[numRectangleIndices];
        scribePlateIndices = new uint[numScribePlateIndices];

        numFork1Vertices = 4;
        numFork1Indices = 6;
        fork1Positions = new float[numFork1Vertices * 2];
        initialFork1Positions = new float[numFork1Vertices * 2];
        fork1Indices = new uint[numFork1Indices];

        numfork2Vertices = 4;
        numfork2Indices = 6;
        fork2Positions = new float[numfork2Vertices * 2];
        initialFork2Positions = new float[numfork2Vertices * 2];
        fork2Indices = new uint[numfork2Indices];
    }

    ~RobotArmEndEffector()
    {
        DeleteBuffers(circleVao, circleVbo, circleEbo);
        DeleteBuffers(rectangleVao, rectangleVbo, rectangleEbo);
        DeleteBuffers(scribePlateVao, scribePlateVbo, scribePlateEbo);
        DeleteBuffers(fork1Ebo, fork1Vbo, fork1Ebo);
        DeleteBuffers(fork2Ebo, fork2Vbo, fork2Ebo);
    }

    public void Initialize()
    {
        InitializeCircleVertices();
        InitializeRectangleVertices();
        InitializeScribePlateVertices(0.1f, 0.0f, 0.0f, -1.0f, 1.0f, 21); // Adjusted 'a' value to 0.1
        InitializeIndices();
        InitializeScribePlateIndices();
        InitializeFork1Vertices();
        InitializeFork2Vertices();

        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(scribePlatePositions, numScribePlateVertices, -scale * 1.1f, 0.0f); // Adjusted offset to move scribePlate to the left
       


        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialScribePlatePositions, numScribePlateVertices, -scale * 1.1f, 0.0f); // Adjusted offset to move scribePlate to the left
        

        CreateBuffer(ref circleVao, ref circleVbo, ref circleEbo, circlePositions, circleIndices, numCircleVertices, numCircleIndices);
        CreateBuffer(ref rectangleVao, ref rectangleVbo, ref rectangleEbo, rectanglePositions, rectangleIndices, numRectangleVertices, numRectangleIndices);
        CreateBuffer(ref scribePlateVao, ref scribePlateVbo, ref scribePlateEbo, scribePlatePositions, scribePlateIndices, numScribePlateVertices, numScribePlateIndices);
        CreateBuffer(ref fork1Vao, ref fork1Vbo, ref fork1Ebo, fork1Positions, fork1Indices, numFork1Vertices, numFork1Indices);
        CreateBuffer(ref fork2Vao, ref fork2Vbo, ref fork2Ebo, fork2Positions, fork2Indices, numfork2Vertices, numfork2Indices);
    }

    public void UpdateRotation(float angle, float centerX, float centerY)
    {
        ApplyRotation(circlePositions, initialCirclePositions, numCircleVertices, angle, centerX, centerY);
        ApplyRotation(rectanglePositions, initialRectanglePositions, numRectangleVertices, angle, centerX, centerY);
        ApplyRotation(scribePlatePositions, initialScribePlatePositions, numScribePlateVertices, angle, centerX, centerY);
        ApplyRotation(fork1Positions, initialFork1Positions, numFork1Vertices, angle, centerX, centerY);
        ApplyRotation(fork2Positions, initialFork2Positions, numfork2Vertices, angle, centerX, centerY);

        UpdateBuffer(circleVbo, circlePositions, numCircleVertices);
        UpdateBuffer(rectangleVbo, rectanglePositions, numRectangleVertices);
        UpdateBuffer(scribePlateVbo, scribePlatePositions, numScribePlateVertices);
        UpdateBuffer(fork1Vbo, fork1Positions, numFork1Vertices);
        UpdateBuffer(fork2Vbo, fork2Positions, numfork2Vertices);
    }

    public void TranslateArbitrary(float[] positions, int numVertices, float offsetX, float offsetY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            positions[i] += offsetX;
            positions[i + 1] += offsetY;
        }

        UpdateBuffer(positions == circlePositions ? circleVbo : positions == rectanglePositions ? rectangleVbo : scribePlateVbo, positions, numVertices);
    }

    public void TranslateToPosition(float x, float y)
    {
        var redDotPosition = CalculateRedDotPosition();
        float offsetX = x - redDotPosition.Item1;
        float offsetY = y - redDotPosition.Item2;

        TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
        TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
        TranslateArbitrary(scribePlatePositions, numScribePlateVertices, offsetX, offsetY);
    }

    public void Render(Shader shader)
    {
        shader.Bind();

        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            Draw(circleVao, numCircleIndices, PrimitiveType.LineLoop);
            Draw(rectangleVao, numRectangleIndices, PrimitiveType.LineLoop);
            GL.BindVertexArray(scribePlateVao);
            GL.DrawElements(PrimitiveType.LineLoop, numScribePlateVertices, DrawElementsType.UnsignedInt, (IntPtr)(sizeof(uint) * (numScribePlateIndices - numScribePlateVertices)));

            Draw(fork1Vao, numFork1Indices, PrimitiveType.LineLoop);
            Draw(fork2Vao, numfork2Indices, PrimitiveType.LineLoop);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            Draw(circleVao, numCircleIndices, PrimitiveType.Triangles);
            Draw(rectangleVao, numRectangleIndices, PrimitiveType.Triangles);
            Draw(scribePlateVao, numScribePlateIndices - numScribePlateVertices, PrimitiveType.Triangles);

            //GL.Uniform4(location, 1.0f, 0.1f, 0.1f, 1.0f);
            Draw(fork1Vao, numFork1Indices, PrimitiveType.Triangles);

            //GL.Uniform4(location, 0.3f, 1.0f, 0.3f, 1.0f);
            Draw(fork2Vao, numfork2Indices, PrimitiveType.Triangles);
        }
        else
        {
            Console.WriteLine("Uniform location for 'u_Color' is invalid.");
        }

        GL.BindVertexArray(0);
        shader.Unbind();
    }

    private void InitializeCircleVertices()
    {
        int index = 0;
        for (int i = 0; i <= 20; ++i)
        {
            float theta = -PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (0.65f + 0.2f * (float)Math.Cos(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }
    }

    private void InitializeRectangleVertices()
    {
        rectanglePositions[0] = scale * 0.65f;  // Top right
        initialRectanglePositions[0] = rectanglePositions[0];
        rectanglePositions[1] = scale * 0.2f;
        initialRectanglePositions[1] = rectanglePositions[1];
        rectanglePositions[2] = scale * 0.65f;  // Bottom right
        initialRectanglePositions[2] = rectanglePositions[2];
        rectanglePositions[3] = scale * -0.2f;
        initialRectanglePositions[3] = rectanglePositions[3];
        rectanglePositions[4] = scale * -0.65f; // Bottom left
        initialRectanglePositions[4] = rectanglePositions[4];
        rectanglePositions[5] = scale * -0.2f;
        initialRectanglePositions[5] = rectanglePositions[5];
        rectanglePositions[6] = scale * -0.65f; // Top left
        initialRectanglePositions[6] = rectanglePositions[6];
        rectanglePositions[7] = scale * 0.2f;
        initialRectanglePositions[7] = rectanglePositions[7];
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

    private void InitializeIndices()
    {
        int index = 0;
        for (int i = 0; i < 20; ++i)
        {
            circleIndices[index++] = 0;
            circleIndices[index++] = (uint)i;
            circleIndices[index++] = (uint)(i + 1);
        }

        rectangleIndices[0] = 0; // Top right
        rectangleIndices[1] = 1; // Bottom right
        rectangleIndices[2] = 2; // Bottom left
        rectangleIndices[3] = 0; // Top right
        rectangleIndices[4] = 2; // Bottom left
        rectangleIndices[5] = 3; // Top left

        fork1Indices[0] = 0;
        fork1Indices[1] = 1;
        fork1Indices[2] = 2;
        fork1Indices[3] = 0;
        fork1Indices[4] = 2;
        fork1Indices[5] = 3;

        fork2Indices[0] = 0;
        fork2Indices[1] = 1;
        fork2Indices[2] = 2;
        fork2Indices[3] = 0;
        fork2Indices[4] = 2;
        fork2Indices[5] = 3;
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

        // Indices for the outline (top parabola forward)
        for (int i = 0; i < numPoints; ++i)
        {
            scribePlateIndices[index++] = (uint)(i * 2); // top curve
        }

        // Outline (bottom parabola in reverse)
        for (int i = numPoints - 1; i >= 0; --i)
        {
            scribePlateIndices[index++] = (uint)(i * 2 + 1); // bottom curve
        }
    }

    public void InitializeFork1Vertices()
    {
        float forkLength = scale * 0.4f;
        float forkHeight = scale * 0.05f;
        float xStart = -scale * 1.7f; // left edge of rectangle
        float yStart = scale * 0.2f;  // just above centerline

        fork1Positions[0] = xStart;               // Top left
        fork1Positions[1] = yStart + forkHeight;
        fork1Positions[2] = xStart;               // Bottom left
        fork1Positions[3] = yStart;
        fork1Positions[4] = xStart + forkLength;  // Bottom right
        fork1Positions[5] = yStart;
        fork1Positions[6] = xStart + forkLength;  // Top right
        fork1Positions[7] = yStart + forkHeight;

        for (int i = 0; i < 8; ++i)
            initialFork1Positions[i] = fork1Positions[i];
    }




    public void InitializeFork2Vertices()
    {
        float forkLength = scale * 0.4f;
        float forkHeight = scale * 0.05f;
        float xStart = -scale * 1.7f; // left edge of rectangle
        float yStart = -scale * 0.25f;  // just below centerline

        fork2Positions[0] = xStart;               // Top left
        fork2Positions[1] = yStart + forkHeight;
        fork2Positions[2] = xStart;               // Bottom left
        fork2Positions[3] = yStart;
        fork2Positions[4] = xStart + forkLength;  // Bottom right
        fork2Positions[5] = yStart;
        fork2Positions[6] = xStart + forkLength;  // Top right
        fork2Positions[7] = yStart + forkHeight;

        for (int i = 0; i < 8; ++i)
            initialFork2Positions[i] = fork2Positions[i];
    }





    private void CreateBuffer(ref int vao, ref int vbo, ref int ebo, float[] positions, uint[] indices, int numVertices, int numIndices)
    {
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numVertices * 2, positions, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numIndices, indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);
    }

    private void UpdateBuffer(int vbo, float[] positions, int numVertices)
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numVertices * 2, positions);
    }

    private void ApplyRotation(float[] positions, float[] initialPositions, int numVertices, float angle, float centerX, float centerY)
    {
        float cosA = (float)Math.Cos(-angle);
        float sinA = (float)Math.Sin(-angle);

        for (int i = 0; i < numVertices * 2; i += 2)
        {
            float x = initialPositions[i] - centerX;
            float y = initialPositions[i + 1] - centerY;

            positions[i] = cosA * x - sinA * y + centerX;
            positions[i + 1] = sinA * x + cosA * y + centerY;
        }
    }

    private void Draw(int vao, int numIndices, PrimitiveType mode)
    {
        GL.BindVertexArray(vao);
        GL.DrawElements(mode, numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
    }

    private void DeleteBuffers(int vao, int vbo, int ebo)
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
    }

    private void TranslateToCenter(float[] positions, int numVertices, float offsetX, float offsetY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            positions[i] += offsetX;
            positions[i + 1] += offsetY;
        }
    }

    private Tuple<float, float> CalculateRedDotPosition()
    {
        return CalculateRectangleHeightMidpoint();
    }

    private Tuple<float, float> CalculateRectangleHeightMidpoint()
    {
        float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
        float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
        return Tuple.Create(midpointX, midpointY);
    }
}
