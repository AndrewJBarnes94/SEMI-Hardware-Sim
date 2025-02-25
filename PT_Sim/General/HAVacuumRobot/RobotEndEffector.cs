using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

public class RobotEndEffector
{
    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;
    private int squareVao, squareVbo, squareEbo;
    private int rightTriangleVao1, rightTriangleVbo1, rightTriangleEbo1;
    private int rightTriangleVao2, rightTriangleVbo2, rightTriangleEbo2;
    private int rightTriangleVao3, rightTriangleVbo3, rightTriangleEbo3;
    private int rightTriangleVao4, rightTriangleVbo4, rightTriangleEbo4;
    private int smallRectangleVao1, smallRectangleVbo1, smallRectangleEbo1;
    private int smallRectangleVao2, smallRectangleVbo2, smallRectangleEbo2;

    private float scale;
    private float[] circlePositions, rectanglePositions, squarePositions;
    private float[] rightTrianglePositions1, rightTrianglePositions2, rightTrianglePositions3, rightTrianglePositions4;
    private float[] smallRectanglePositions1, smallRectanglePositions2;

    private int[] circleIndices, rectangleIndices, squareIndices;
    private int[] rightTriangleIndices1, rightTriangleIndices2, rightTriangleIndices3, rightTriangleIndices4;
    private int[] smallRectangleIndices1, smallRectangleIndices2;

    private int numCircleVertices = 21;
    private int numRectangleVertices = 4;
    private int numSquareVertices = 4;
    private int numRightTriangleVertices = 3;
    private int numSmallRectangleVertices = 4;

    private int numCircleIndices = 3 * 20;
    private int numRectangleIndices = 6;
    private int numSquareIndices = 6;
    private int numRightTriangleIndices = 3;
    private int numSmallRectangleIndices = 6;

    public RobotEndEffector(float scale)
    {
        this.scale = scale;
        circlePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];
        squarePositions = new float[numSquareVertices * 2];
        rightTrianglePositions1 = new float[numRightTriangleVertices * 2];
        rightTrianglePositions2 = new float[numRightTriangleVertices * 2];
        rightTrianglePositions3 = new float[numRightTriangleVertices * 2];
        rightTrianglePositions4 = new float[numRightTriangleVertices * 2];
        smallRectanglePositions1 = new float[numSmallRectangleVertices * 2];
        smallRectanglePositions2 = new float[numSmallRectangleVertices * 2];

        circleIndices = new int[numCircleIndices];
        rectangleIndices = new int[numRectangleIndices];
        squareIndices = new int[numSquareIndices];
        rightTriangleIndices1 = new int[numRightTriangleIndices];
        rightTriangleIndices2 = new int[numRightTriangleIndices];
        rightTriangleIndices3 = new int[numRightTriangleIndices];
        rightTriangleIndices4 = new int[numRightTriangleIndices];
        smallRectangleIndices1 = new int[numSmallRectangleIndices];
        smallRectangleIndices2 = new int[numSmallRectangleIndices];
    }

    public void Dispose()
    {
        int[] buffers = { circleVbo, rectangleVbo, squareVbo, rightTriangleVbo1, rightTriangleVbo2, rightTriangleVbo3, rightTriangleVbo4, smallRectangleVbo1, smallRectangleVbo2 };
        int[] elements = { circleEbo, rectangleEbo, squareEbo, rightTriangleEbo1, rightTriangleEbo2, rightTriangleEbo3, rightTriangleEbo4, smallRectangleEbo1, smallRectangleEbo2 };
        int[] vaos = { circleVao, rectangleVao, squareVao, rightTriangleVao1, rightTriangleVao2, rightTriangleVao3, rightTriangleVao4, smallRectangleVao1, smallRectangleVao2 };

        foreach (var buffer in buffers) GL.DeleteBuffer(buffer);
        foreach (var element in elements) GL.DeleteBuffer(element);
        foreach (var vao in vaos) GL.DeleteVertexArray(vao);
    }

    public void Initialize()
    {
        int index = 0;

        // Circle vertices (-π/2 to π/2)
        for (int i = 0; i <= 20; i++)
        {
            float theta = (float)(-Math.PI / 2 + Math.PI * i / 20);
            circlePositions[index++] = scale * (0.4f + 0.2f * (float)Math.Cos(theta));
            circlePositions[index++] = scale * (0.2f * (float)Math.Sin(theta));
        }

        // Rectangle vertices
        float[] rectVerts = {
            0.4f,  0.2f,  // Top right
            0.4f, -0.2f,  // Bottom right
           -0.4f, -0.2f,  // Bottom left
           -0.4f,  0.2f   // Top left
        };
        Array.Copy(rectVerts, rectanglePositions, rectVerts.Length);

        // Translate all elements to center
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

        index = 0;

        // Circle indices
        for (int i = 0; i < 20; i++)
        {
            circleIndices[index++] = 0;
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
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.Triangles, numCircleIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.Triangles, numRectangleIndices, DrawElementsType.UnsignedInt, 0);
        }

        GL.BindVertexArray(0);
        shader.Unbind();
    }
}
