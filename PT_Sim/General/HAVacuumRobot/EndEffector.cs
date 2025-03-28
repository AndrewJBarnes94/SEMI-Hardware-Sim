using System;
using OpenTK.Graphics.OpenGL;

public class RobotArmEndEffector
{
    private float scale;

    private int numCircleVertices, numRectangleVertices;
    private int numCircleIndices, numRectangleIndices;

    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;

    private float[] circlePositions, rectanglePositions;
    private float[] initialCirclePositions, initialRectanglePositions;

    private uint[] circleIndices, rectangleIndices;

    private const float PI = 3.14159265358979323846f;

    public RobotArmEndEffector(float scale)
    {
        this.scale = scale / 3;

        numCircleVertices = 21;
        numRectangleVertices = 4;

        numCircleIndices = 3 * 20;
        numRectangleIndices = 6;

        // Initialize arrays to prevent `NullReferenceException`
        circlePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];

        initialCirclePositions = new float[numCircleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];

        circleIndices = new uint[numCircleIndices];
        rectangleIndices = new uint[numRectangleIndices];
    }

    ~RobotArmEndEffector()
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

        // Left half-circle vertices (from -π/2 to π/2)
        for (int i = 0; i <= 20; ++i)
        {
            float theta = -PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (0.7f + 0.2f * (float)Math.Cos(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        // Rectangle vertices
        rectanglePositions[0] = scale * 0.7f;  // Top right
        initialRectanglePositions[0] = rectanglePositions[0];
        rectanglePositions[1] = scale * 0.2f;
        initialRectanglePositions[1] = rectanglePositions[1];
        rectanglePositions[2] = scale * 0.7f;  // Bottom right
        initialRectanglePositions[2] = rectanglePositions[2];
        rectanglePositions[3] = scale * -0.2f;
        initialRectanglePositions[3] = rectanglePositions[3];
        rectanglePositions[4] = scale * -0.7f; // Bottom left
        initialRectanglePositions[4] = rectanglePositions[4];
        rectanglePositions[5] = scale * -0.2f;
        initialRectanglePositions[5] = rectanglePositions[5];
        rectanglePositions[6] = scale * -0.7f; // Top left
        initialRectanglePositions[6] = rectanglePositions[6];
        rectanglePositions[7] = scale * 0.2f;
        initialRectanglePositions[7] = rectanglePositions[7];

        // Translate the entire geometry to center
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

        index = 0;

        // Left half-circle indices
        for (int i = 0; i < 20; ++i)
        {
            circleIndices[index++] = 0;
            circleIndices[index++] = (uint)i;
            circleIndices[index++] = (uint)(i + 1);
        }

        // Rectangle indices
        rectangleIndices[0] = 0; // Top right
        rectangleIndices[1] = 1; // Bottom right
        rectangleIndices[2] = 2; // Bottom left
        rectangleIndices[3] = 0; // Top right
        rectangleIndices[4] = 2; // Bottom left
        rectangleIndices[5] = 3; // Top left

        // Vertex Array Object and Buffer for circles
        GL.GenVertexArrays(1, out circleVao);
        GL.GenBuffers(1, out circleVbo);
        GL.GenBuffers(1, out circleEbo);

        // Initialize VAO and VBO for circles
        GL.BindVertexArray(circleVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numCircleVertices * 2, circlePositions, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, circleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numCircleIndices, circleIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for rectangle
        GL.GenVertexArrays(1, out rectangleVao);
        GL.GenBuffers(1, out rectangleVbo);
        GL.GenBuffers(1, out rectangleEbo);

        // Initialize VAO and VBO for rectangle
        GL.BindVertexArray(rectangleVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rectangleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numRectangleVertices * 2, rectanglePositions, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rectangleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numRectangleIndices, rectangleIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);
    }

    public void UpdateRotation(float angle, float centerX, float centerY)
    {
        for (int i = 0; i < numCircleVertices * 2; i += 2)
        {
            float x = initialCirclePositions[i] - centerX;
            float y = initialCirclePositions[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            circlePositions[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            circlePositions[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRectangleVertices * 2; i += 2)
        {
            float x = initialRectanglePositions[i] - centerX;
            float y = initialRectanglePositions[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rectanglePositions[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            rectanglePositions[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        // Update the vertex buffer with the new positions
        GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numCircleVertices * 2, circlePositions);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rectangleVbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRectangleVertices * 2, rectanglePositions);
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

    public void TranslateToPosition(float x, float y)
    {
        // Calculate the offset needed to translate the appendage to the specified position
        var redDotPosition = CalculateRedDotPosition();
        float offsetX = x - redDotPosition.Item1;
        float offsetY = y - redDotPosition.Item2;

        // Translate the entire geometry
        TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
        TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
    }

    public void Render(Shader shader)
    {
        shader.Bind(); // Ensure the shader is bound

        int location = shader.GetUniformLocation("u_Color");

        if (location != -1) // Check if the uniform location is valid
        {
            // Draw the outline
            GL.LineWidth(2.0f); // Set the line width for the outline
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f); // Set color to black

            // Draw the circles outline
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numCircleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the rectangle outline
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numRectangleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the fill
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Set color to metallic gray/silver

            // Draw the circles fill
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.Triangles, numCircleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the rectangle fill
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.Triangles, numRectangleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
        else
        {
            Console.WriteLine("Uniform location for 'u_Color' is invalid.");
        }

        GL.BindVertexArray(0); // Unbind the VAO
        shader.Unbind(); // Unbind the shader program
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
        // Calculate the red dot position based on the rectangle's height midpoint
        return CalculateRectangleHeightMidpoint();
    }

    private Tuple<float, float> CalculateRectangleHeightMidpoint()
    {
        // Midpoint of the rectangle's height on the left side
        float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
        float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
        return Tuple.Create(midpointX, midpointY);
    }
}
