using System;
using OpenTK.Graphics.OpenGL;

public class RobotArmEndEffector
{
    private const float PI = 3.14159265358979323846f;
    private readonly float scale;

    private Geometry circle, rectangle, scribePlate, fork1, fork2;

    public RobotArmEndEffector(float scale)
    {
        this.scale = scale / 3;

        circle = new Geometry(21, 3 * 20);
        rectangle = new Geometry(4, 6);
        scribePlate = new Geometry(42, 6 * 20 + 2 * 21);
        fork1 = new Geometry(4, 6);
        fork2 = new Geometry(4, 6);
    }

    ~RobotArmEndEffector()
    {
        circle.DeleteBuffers();
        rectangle.DeleteBuffers();
        scribePlate.DeleteBuffers();
        fork1.DeleteBuffers();
        fork2.DeleteBuffers();
    }

    public void Initialize()
    {
        InitializeCircle();
        InitializeRectangle();
        InitializeScribePlate();
        InitializeFork(fork1, -1.7f, 0.25f, -1.3f, 0.2f);
        InitializeFork(fork2, -1.7f, -0.2f, -1.3f, -0.25f);

        // Apply translations to geometries
        TranslateGeometry(circle, -scale * 0.4f * 2, 0.0f);
        TranslateGeometry(rectangle, -scale * 0.4f * 2, 0.0f);
        TranslateGeometry(scribePlate, -scale * 1.1f * 2, 0.0f);

        // Ensure buffers are updated after translations
        circle.UpdateBuffer();
        rectangle.UpdateBuffer();
        scribePlate.UpdateBuffer();
        fork1.UpdateBuffer();
        fork2.UpdateBuffer();

        // Create buffers for rendering
        circle.CreateBuffer();
        rectangle.CreateBuffer();
        scribePlate.CreateBuffer();
        fork1.CreateBuffer();
        fork2.CreateBuffer();
    }


    public void UpdateRotation(float angle, float centerX, float centerY)
    {
        RotateGeometry(circle, angle, centerX, centerY);
        RotateGeometry(rectangle, angle, centerX, centerY);
        RotateGeometry(scribePlate, angle, centerX, centerY);
        RotateGeometry(fork1, angle, centerX, centerY);
        RotateGeometry(fork2, angle, centerX, centerY);
    }

    public void TranslateToPosition(float x, float y)
    {
        // Define custom offsets for each geometry
        float circleOffsetX = x - 0.0f;  // Example: Move circle to x - 0.5
        float circleOffsetY = y + 0.0f;  // Example: Move circle to y + 0.2

        float rectangleOffsetX = x - 0.0f;  // Example: Move rectangle to x - 0.3
        float rectangleOffsetY = y;         // Example: Keep rectangle aligned with y

        float scribePlateOffsetX = x - 0.0f;  // Example: Move scribe plate further left
        float scribePlateOffsetY = y;         // Example: Keep scribe plate aligned with y

        float fork1OffsetX = x - 0.0f;  // Example: Move fork1 to x - 0.7
        float fork1OffsetY = y + 0.0f;  // Example: Move fork1 slightly up

        float fork2OffsetX = x - 0.0f;  // Example: Move fork2 to x - 0.7
        float fork2OffsetY = y - 0.0f;  // Example: Move fork2 slightly down

        // Apply translations to each geometry
        TranslateGeometry(circle, circleOffsetX, circleOffsetY);
        TranslateGeometry(rectangle, rectangleOffsetX, rectangleOffsetY);
        TranslateGeometry(scribePlate, scribePlateOffsetX, scribePlateOffsetY);
        TranslateGeometry(fork1, fork1OffsetX, fork1OffsetY);
        TranslateGeometry(fork2, fork2OffsetX, fork2OffsetY);
    }


    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            circle.Render(PrimitiveType.LineLoop);
            rectangle.Render(PrimitiveType.LineLoop);
            scribePlate.RenderOutline();
            fork1.Render(PrimitiveType.LineLoop);
            fork2.Render(PrimitiveType.LineLoop);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            circle.Render(PrimitiveType.Triangles);
            rectangle.Render(PrimitiveType.Triangles);
            scribePlate.Render(PrimitiveType.Triangles);
            fork1.Render(PrimitiveType.Triangles);
            fork2.Render(PrimitiveType.Triangles);
        }
        else
        {
            Console.WriteLine("Uniform location for 'u_Color' is invalid.");
        }

        shader.Unbind();
    }

    private void InitializeCircle()
    {
        for (int i = 0; i <= 20; ++i)
        {
            float theta = -PI / 2 + PI * i / 20;
            circle.AddVertex(scale * (0.65f + 0.2f * (float)Math.Cos(theta)), scale * (0.2f * (float)Math.Sin(theta)));
        }
        circle.AddIndicesForFan();
    }

    private void InitializeRectangle()
    {
        rectangle.AddVertex(scale * 0.65f, scale * 0.2f);  // Top right
        rectangle.AddVertex(scale * 0.65f, scale * -0.2f); // Bottom right
        rectangle.AddVertex(scale * -0.65f, scale * -0.2f); // Bottom left
        rectangle.AddVertex(scale * -0.65f, scale * 0.2f);  // Top left
        rectangle.AddIndicesForQuad();
    }

    private void InitializeScribePlate()
    {
        float a = 0.1f, b = 0.0f, c = 0.0f, startX = -1.0f, endX = 1.0f;
        int numPoints = 21;
        float step = (endX - startX) / (numPoints - 1);
        float scribeScaleX = 0.25f, scribeScaleY = 1.0f;

        for (int i = 0; i < numPoints; ++i)
        {
            float x = startX + i * step;
            float y = a * x * x + b * x + c;

            scribePlate.AddVertex(-y * scale * scribeScaleY, -x * scale * scribeScaleX); // First parabola
            scribePlate.AddVertex(-(y - 0.2f) * scale * scribeScaleY, -x * scale * scribeScaleX); // Second parabola
        }
        scribePlate.AddIndicesForScribePlate(numPoints);
    }

    private void InitializeFork(Geometry fork, float leftX, float topY, float rightX, float bottomY)
    {
        fork.AddVertex(leftX * scale, topY * scale);    // Top left
        fork.AddVertex(leftX * scale, bottomY * scale); // Bottom left
        fork.AddVertex(rightX * scale, bottomY * scale); // Bottom right
        fork.AddVertex(rightX * scale, topY * scale);   // Top right
        fork.AddIndicesForQuad();
    }

    private void TranslateGeometry(Geometry geometry, float offsetX, float offsetY)
    {
        geometry.Translate(offsetX, offsetY);
    }

    private void RotateGeometry(Geometry geometry, float angle, float centerX, float centerY)
    {
        geometry.Rotate(angle, centerX, centerY);
    }

    private (float, float) CalculateRectangleMidpoint()
    {
        return rectangle.CalculateMidpoint();
    }

    private class Geometry
    {
        private readonly int numVertices;
        private readonly int numIndices;
        private float[] positions;
        private float[] initialPositions;
        private uint[] indices;
        private int vao, vbo, ebo;
        private int vertexCount = 0, indexCount = 0;

        public Geometry(int numVertices, int numIndices)
        {
            this.numVertices = numVertices;
            this.numIndices = numIndices;
            positions = new float[numVertices * 2];
            initialPositions = new float[numVertices * 2];
            indices = new uint[numIndices];
        }

        public void AddVertex(float x, float y)
        {
            positions[vertexCount] = x;
            initialPositions[vertexCount++] = x;
            positions[vertexCount] = y;
            initialPositions[vertexCount++] = y;
        }

        public void AddIndex(uint index)
        {
            indices[indexCount++] = index;
        }

        public void AddIndicesForFan()
        {
            for (uint i = 1; i < numVertices - 1; ++i)
            {
                AddIndex(0);
                AddIndex(i);
                AddIndex(i + 1);
            }
        }

        public void AddIndicesForQuad()
        {
            AddIndex(0); AddIndex(1); AddIndex(2);
            AddIndex(0); AddIndex(2); AddIndex(3);
        }

        public void AddIndicesForScribePlate(int numPoints)
        {
            for (int i = 0; i < numPoints - 1; ++i)
            {
                uint topLeft = (uint)(i * 2);
                uint bottomLeft = (uint)(i * 2 + 1);
                uint topRight = (uint)(i * 2 + 2);
                uint bottomRight = (uint)(i * 2 + 3);

                AddIndex(topLeft); AddIndex(bottomLeft); AddIndex(topRight);
                AddIndex(topRight); AddIndex(bottomLeft); AddIndex(bottomRight);
            }

            for (int i = 0; i < numPoints; ++i) AddIndex((uint)(i * 2));
            for (int i = numPoints - 1; i >= 0; --i) AddIndex((uint)(i * 2 + 1));
        }

        public void Translate(float offsetX, float offsetY)
        {
            for (int i = 0; i < positions.Length; i += 2)
            {
                positions[i] += offsetX;
                positions[i + 1] += offsetY;
            }
            UpdateBuffer();
        }

        public void Rotate(float angle, float centerX, float centerY)
        {
            float cosA = (float)Math.Cos(-angle);
            float sinA = (float)Math.Sin(-angle);

            for (int i = 0; i < positions.Length; i += 2)
            {
                float x = initialPositions[i] - centerX;
                float y = initialPositions[i + 1] - centerY;

                positions[i] = cosA * x - sinA * y + centerX;
                positions[i + 1] = sinA * x + cosA * y + centerY;
            }
            UpdateBuffer();
        }

        public (float, float) CalculateMidpoint()
        {
            float midpointX = (positions[0] + positions[4]) / 2;
            float midpointY = (positions[1] + positions[5]) / 2;
            return (midpointX, midpointY);
        }

        public void CreateBuffer()
        {
            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);
            GL.GenBuffers(1, out ebo);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * indices.Length, indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
            GL.EnableVertexAttribArray(0);
        }

        public void UpdateBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * positions.Length, positions);
        }

        public void Render(PrimitiveType mode)
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(mode, numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void RenderOutline()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numVertices, DrawElementsType.UnsignedInt, (IntPtr)(sizeof(uint) * (numIndices - numVertices)));
        }

        public void DeleteBuffers()
        {
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
        }
    }
}
