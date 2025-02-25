using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

public class RobotArmSegment
{
    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;
    private float scale;
    private float[] circlePositions;
    private float[] initialCirclePositions;
    private float[] rectanglePositions;
    private float[] initialRectanglePositions;
    private int[] circleIndices;
    private int[] rectangleIndices;

    private int numCircleVertices = 2 * (20 + 1);
    private int numRectangleVertices = 4;
    private int numCircleIndices = 2 * 3 * 20;
    private int numRectangleIndices = 6;

    public RobotArmSegment(float scale)
    {
        this.scale = scale;
        circlePositions = new float[numCircleVertices * 2];
        initialCirclePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];
        circleIndices = new int[numCircleIndices];
        rectangleIndices = new int[numRectangleIndices];
    }

    public void Dispose()
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

        // Left half-circle vertices (-π/2 to π/2)
        for (int i = 0; i <= 20; i++)
        {
            float theta = (float)(-Math.PI / 2 + Math.PI * i / 20);
            circlePositions[index] = scale * (0.4f + 0.2f * (float)Math.Cos(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        // Right half-circle vertices (π/2 to 3π/2)
        for (int i = 0; i <= 20; i++)
        {
            float theta = (float)(Math.PI / 2 + Math.PI * i / 20);
            circlePositions[index] = scale * (-0.4f + 0.2f * (float)Math.Cos(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        // Rectangle vertices
        float[] rectVerts = {
            0.4f,  0.2f,  // Top right
            0.4f, -0.2f,  // Bottom right
           -0.4f, -0.2f,  // Bottom left
           -0.4f,  0.2f   // Top left
        };

        for (int i = 0; i < rectVerts.Length; i++)
        {
            rectanglePositions[i] = scale * rectVerts[i];
            initialRectanglePositions[i] = rectanglePositions[i];
        }

        // Center translation
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

        index = 0;

        // Left half-circle indices
        for (int i = 0; i < 20; i++)
        {
            circleIndices[index++] = 0;
            circleIndices[index++] = i;
            circleIndices[index++] = i + 1;
        }

        // Right half-circle indices
        for (int i = 21; i < 41; i++)
        {
            circleIndices[index++] = 21;
            circleIndices[index++] = i;
            circleIndices[index++] = i + 1;
        }

        // Rectangle indices
        rectangleIndices = new int[] { 0, 1, 2, 0, 2, 3 };

        // Initialize OpenGL buffers
        circleVao = GL.GenVertexArray();
        circleVbo = GL.GenBuffer();
        circleEbo = GL.GenBuffer();
        GL.BindVertexArray(circleVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, circlePositions.Length * sizeof(float), circlePositions, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, circleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, circleIndices.Length * sizeof(int), circleIndices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        rectangleVao = GL.GenVertexArray();
        rectangleVbo = GL.GenBuffer();
        rectangleEbo = GL.GenBuffer();
        GL.BindVertexArray(rectangleVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rectangleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, rectanglePositions.Length * sizeof(float), rectanglePositions, BufferUsageHint.DynamicDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rectangleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, rectangleIndices.Length * sizeof(int), rectangleIndices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public void TranslateToCenter(float[] positions, int numVertices, float offsetX, float offsetY)
    {
        for (int i = 0; i < numVertices * 2; i += 2)
        {
            positions[i] += offsetX;
            positions[i + 1] += offsetY;
        }
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            // Draw outline
            GL.LineWidth(5.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numCircleIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numRectangleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw fill
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.Triangles, numCircleIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.Triangles, numRectangleIndices, DrawElementsType.UnsignedInt, 0);
        }
        else
        {
            Console.WriteLine("Uniform location for 'u_Color' is invalid.");
        }

        GL.BindVertexArray(0);
        shader.Unbind();
    }
}

