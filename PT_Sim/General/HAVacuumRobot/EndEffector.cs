using System;
using OpenTK.Graphics.OpenGL;

public class RobotArmEndEffector
{
    private float scale;

    private int numCircleVertices, numRectangleVertices, numSquareVertices;
    private int numRightTriangleVertices1, numRightTriangleVertices2, numRightTriangleVertices3, numRightTriangleVertices4;
    private int numSmallRectangleVertices1, numSmallRectangleVertices2;

    private int numCircleIndices, numRectangleIndices, numSquareIndices;
    private int numRightTriangleIndices1, numRightTriangleIndices2, numRightTriangleIndices3, numRightTriangleIndices4;
    private int numSmallRectangleIndices1, numSmallRectangleIndices2;

    private int circleVao, circleVbo, circleEbo;
    private int rectangleVao, rectangleVbo, rectangleEbo;
    private int squareVao, squareVbo, squareEbo;
    private int rightTriangleVao1, rightTriangleVbo1, rightTriangleEbo1;
    private int rightTriangleVao2, rightTriangleVbo2, rightTriangleEbo2;
    private int rightTriangleVao3, rightTriangleVbo3, rightTriangleEbo3;
    private int rightTriangleVao4, rightTriangleVbo4, rightTriangleEbo4;
    private int smallRectangleVao1, smallRectangleVbo1, smallRectangleEbo1;
    private int smallRectangleVao2, smallRectangleVbo2, smallRectangleEbo2;

    private float[] circlePositions, rectanglePositions, squarePositions;
    private float[] rightTrianglePositions1, rightTrianglePositions2, rightTrianglePositions3, rightTrianglePositions4;
    private float[] smallRectanglePositions1, smallRectanglePositions2;

    private float[] initialCirclePositions, initialRectanglePositions, initialSquarePositions;
    private float[] initialRightTrianglePositions1, initialRightTrianglePositions2, initialRightTrianglePositions3, initialRightTrianglePositions4;
    private float[] initialSmallRectanglePositions1, initialSmallRectanglePositions2;

    private uint[] circleIndices, rectangleIndices, squareIndices;
    private uint[] rightTriangleIndices1, rightTriangleIndices2, rightTriangleIndices3, rightTriangleIndices4;
    private uint[] smallRectangleIndices1, smallRectangleIndices2;

    private const float PI = 3.14159265358979323846f;

    public RobotArmEndEffector(float scale)
    {
        this.scale = scale / 3;

        numCircleVertices = 21;
        numRectangleVertices = 4;
        numSquareVertices = 4;
        numRightTriangleVertices1 = 3;
        numRightTriangleVertices2 = 3;
        numRightTriangleVertices3 = 3;
        numRightTriangleVertices4 = 3;
        numSmallRectangleVertices1 = 4;
        numSmallRectangleVertices2 = 4;

        numCircleIndices = 3 * 20;
        numRectangleIndices = 6;
        numSquareIndices = 6;
        numRightTriangleIndices1 = 3;
        numRightTriangleIndices2 = 3;
        numRightTriangleIndices3 = 3;
        numRightTriangleIndices4 = 3;
        numSmallRectangleIndices1 = 6;
        numSmallRectangleIndices2 = 6;

        // Initialize arrays to prevent `NullReferenceException`
        circlePositions = new float[numCircleVertices * 2];
        rectanglePositions = new float[numRectangleVertices * 2];
        squarePositions = new float[numSquareVertices * 2];
        rightTrianglePositions1 = new float[numRightTriangleVertices1 * 2];
        rightTrianglePositions2 = new float[numRightTriangleVertices2 * 2];
        rightTrianglePositions3 = new float[numRightTriangleVertices3 * 2];
        rightTrianglePositions4 = new float[numRightTriangleVertices4 * 2];
        smallRectanglePositions1 = new float[numSmallRectangleVertices1 * 2];
        smallRectanglePositions2 = new float[numSmallRectangleVertices2 * 2];

        initialCirclePositions = new float[numCircleVertices * 2];
        initialRectanglePositions = new float[numRectangleVertices * 2];
        initialSquarePositions = new float[numSquareVertices * 2];
        initialRightTrianglePositions1 = new float[numRightTriangleVertices1 * 2];
        initialRightTrianglePositions2 = new float[numRightTriangleVertices2 * 2];
        initialRightTrianglePositions3 = new float[numRightTriangleVertices3 * 2];
        initialRightTrianglePositions4 = new float[numRightTriangleVertices4 * 2];
        initialSmallRectanglePositions1 = new float[numSmallRectangleVertices1 * 2];
        initialSmallRectanglePositions2 = new float[numSmallRectangleVertices2 * 2];

        circleIndices = new uint[numCircleIndices];
        rectangleIndices = new uint[numRectangleIndices];
        squareIndices = new uint[numSquareIndices];
        rightTriangleIndices1 = new uint[numRightTriangleIndices1];
        rightTriangleIndices2 = new uint[numRightTriangleIndices2];
        rightTriangleIndices3 = new uint[numRightTriangleIndices3];
        rightTriangleIndices4 = new uint[numRightTriangleIndices4];
        smallRectangleIndices1 = new uint[numSmallRectangleIndices1];
        smallRectangleIndices2 = new uint[numSmallRectangleIndices2];
    }

    ~RobotArmEndEffector()
    {
        GL.DeleteVertexArray(circleVao);
        GL.DeleteBuffer(circleVbo);
        GL.DeleteBuffer(circleEbo);

        GL.DeleteVertexArray(rectangleVao);
        GL.DeleteBuffer(rectangleVbo);
        GL.DeleteBuffer(rectangleEbo);

        GL.DeleteVertexArray(squareVao);
        GL.DeleteBuffer(squareVbo);
        GL.DeleteBuffer(squareEbo);

        GL.DeleteBuffer(rightTriangleVbo1);
        GL.DeleteBuffer(rightTriangleEbo1);
        GL.DeleteVertexArray(rightTriangleVao1);

        GL.DeleteBuffer(rightTriangleVbo2);
        GL.DeleteBuffer(rightTriangleEbo2);
        GL.DeleteVertexArray(rightTriangleVao2);

        GL.DeleteBuffer(rightTriangleVbo3);
        GL.DeleteBuffer(rightTriangleEbo3);
        GL.DeleteVertexArray(rightTriangleVao3);

        GL.DeleteBuffer(rightTriangleVbo4);
        GL.DeleteBuffer(rightTriangleEbo4);
        GL.DeleteVertexArray(rightTriangleVao4);

        GL.DeleteBuffer(smallRectangleVbo1);
        GL.DeleteBuffer(smallRectangleEbo1);
        GL.DeleteVertexArray(smallRectangleVao1);

        GL.DeleteBuffer(smallRectangleVbo2);
        GL.DeleteBuffer(smallRectangleEbo2);
        GL.DeleteVertexArray(smallRectangleVao2);
    }

    public void Initialize()
    {
        int index = 0;

        // Left half-circle vertices (from -?/2 to ?/2)
        for (int i = 0; i <= 20; ++i)
        {
            float theta = -PI / 2 + PI * i / 20;
            circlePositions[index] = scale * (0.4f + 0.2f * (float)Math.Cos(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
            circlePositions[index] = scale * (0.2f * (float)Math.Sin(theta));
            initialCirclePositions[index] = circlePositions[index];
            index++;
        }

        // Rectangle vertices
        rectanglePositions[0] = scale * 0.4f;  // Top right
        initialRectanglePositions[0] = rectanglePositions[0];
        rectanglePositions[1] = scale * 0.2f;
        initialRectanglePositions[1] = rectanglePositions[1];
        rectanglePositions[2] = scale * 0.4f;  // Bottom right
        initialRectanglePositions[2] = rectanglePositions[2];
        rectanglePositions[3] = scale * -0.2f;
        initialRectanglePositions[3] = rectanglePositions[3];
        rectanglePositions[4] = scale * -0.4f; // Bottom left
        initialRectanglePositions[4] = rectanglePositions[4];
        rectanglePositions[5] = scale * -0.2f;
        initialRectanglePositions[5] = rectanglePositions[5];
        rectanglePositions[6] = scale * -0.4f; // Top left
        initialRectanglePositions[6] = rectanglePositions[6];
        rectanglePositions[7] = scale * 0.2f;
        initialRectanglePositions[7] = rectanglePositions[7];

        // Square vertices
        squarePositions[0] = scale * -0.4f;  // Top right
        initialSquarePositions[0] = squarePositions[0];
        squarePositions[1] = scale * 0.2f;
        initialSquarePositions[1] = squarePositions[1];
        squarePositions[2] = scale * -0.4f; // Bottom right
        initialSquarePositions[2] = squarePositions[2];
        squarePositions[3] = scale * -0.2f;
        initialSquarePositions[3] = squarePositions[3];
        squarePositions[4] = scale * -0.6f; // Bottom left
        initialSquarePositions[4] = squarePositions[4];
        squarePositions[5] = scale * -0.2f;
        initialSquarePositions[5] = squarePositions[5];
        squarePositions[6] = scale * -0.6f;  // Top left
        initialSquarePositions[6] = squarePositions[6];
        squarePositions[7] = scale * 0.2f;
        initialSquarePositions[7] = squarePositions[7];

        // Right triangle1 vertices
        rightTrianglePositions1[0] = scale * -0.4f;  // Bottom right
        rightTrianglePositions1[1] = scale * 0.2f;
        rightTrianglePositions1[2] = scale * -0.6f;  // Bottom left
        rightTrianglePositions1[3] = scale * 0.2f;
        rightTrianglePositions1[4] = scale * -0.6f;  // Top left
        rightTrianglePositions1[5] = scale * 0.3f;

        initialRightTrianglePositions1[0] = rightTrianglePositions1[0];
        initialRightTrianglePositions1[1] = rightTrianglePositions1[1];
        initialRightTrianglePositions1[2] = rightTrianglePositions1[2];
        initialRightTrianglePositions1[3] = rightTrianglePositions1[3];
        initialRightTrianglePositions1[4] = rightTrianglePositions1[4];
        initialRightTrianglePositions1[5] = rightTrianglePositions1[5];

        // Right triangle2 vertices
        rightTrianglePositions2[0] = scale * -0.4f;  // Top right
        rightTrianglePositions2[1] = scale * -0.2f;
        rightTrianglePositions2[2] = scale * -0.6f;  // Top left
        rightTrianglePositions2[3] = scale * -0.2f;
        rightTrianglePositions2[4] = scale * -0.6f;  // Bottom left
        rightTrianglePositions2[5] = scale * -0.3f;

        initialRightTrianglePositions2[0] = rightTrianglePositions2[0];
        initialRightTrianglePositions2[1] = rightTrianglePositions2[1];
        initialRightTrianglePositions2[2] = rightTrianglePositions2[2];
        initialRightTrianglePositions2[3] = rightTrianglePositions2[3];
        initialRightTrianglePositions2[4] = rightTrianglePositions2[4];
        initialRightTrianglePositions2[5] = rightTrianglePositions2[5];

        // Right triangle3 vertices
        rightTrianglePositions3[0] = scale * -0.6f;  // Top right
        rightTrianglePositions3[1] = scale * 0.2f;
        rightTrianglePositions3[2] = scale * -0.6f;  // Bottom right
        rightTrianglePositions3[3] = scale * 0.1f;
        rightTrianglePositions3[4] = scale * -0.8f;  // Bottom left
        rightTrianglePositions3[5] = scale * 0.2f;

        initialRightTrianglePositions3[0] = rightTrianglePositions3[0];
        initialRightTrianglePositions3[1] = rightTrianglePositions3[1];
        initialRightTrianglePositions3[2] = rightTrianglePositions3[2];
        initialRightTrianglePositions3[3] = rightTrianglePositions3[3];
        initialRightTrianglePositions3[4] = rightTrianglePositions3[4];
        initialRightTrianglePositions3[5] = rightTrianglePositions3[5];

        // Right triangle4 vertices
        rightTrianglePositions4[0] = scale * -0.6f;  // Bottom right
        rightTrianglePositions4[1] = scale * -0.2f;
        rightTrianglePositions4[2] = scale * -0.6f;  // Bottom left
        rightTrianglePositions4[3] = scale * -0.1f;
        rightTrianglePositions4[4] = scale * -0.8f;  // Top left
        rightTrianglePositions4[5] = scale * -0.2f;

        initialRightTrianglePositions4[0] = rightTrianglePositions4[0];
        initialRightTrianglePositions4[1] = rightTrianglePositions4[1];
        initialRightTrianglePositions4[2] = rightTrianglePositions4[2];
        initialRightTrianglePositions4[3] = rightTrianglePositions4[3];
        initialRightTrianglePositions4[4] = rightTrianglePositions4[4];
        initialRightTrianglePositions4[5] = rightTrianglePositions4[5];

        // Small rectangle1 vertices
        smallRectanglePositions1[0] = scale * -0.6f;  // Top right
        smallRectanglePositions1[1] = scale * 0.3f;
        smallRectanglePositions1[2] = scale * -0.6f;  // Bottom right
        smallRectanglePositions1[3] = scale * 0.2f;
        smallRectanglePositions1[4] = scale * -1.0f;  // Bottom left
        smallRectanglePositions1[5] = scale * 0.2f;
        smallRectanglePositions1[6] = scale * -1.0f;  // Top left
        smallRectanglePositions1[7] = scale * 0.3f;

        initialSmallRectanglePositions1[0] = smallRectanglePositions1[0];
        initialSmallRectanglePositions1[1] = smallRectanglePositions1[1];
        initialSmallRectanglePositions1[2] = smallRectanglePositions1[2];
        initialSmallRectanglePositions1[3] = smallRectanglePositions1[3];
        initialSmallRectanglePositions1[4] = smallRectanglePositions1[4];
        initialSmallRectanglePositions1[5] = smallRectanglePositions1[5];
        initialSmallRectanglePositions1[6] = smallRectanglePositions1[6];
        initialSmallRectanglePositions1[7] = smallRectanglePositions1[7];

        // Small rectangle2 vertices
        smallRectanglePositions2[0] = scale * -0.6f;  // Top right
        smallRectanglePositions2[1] = scale * -0.2f;
        smallRectanglePositions2[2] = scale * -0.6f;  // Bottom right
        smallRectanglePositions2[3] = scale * -0.3f;
        smallRectanglePositions2[4] = scale * -1.0f;  // Bottom left
        smallRectanglePositions2[5] = scale * -0.3f;
        smallRectanglePositions2[6] = scale * -1.0f;  // Top left
        smallRectanglePositions2[7] = scale * -0.2f;

        initialSmallRectanglePositions2[0] = smallRectanglePositions2[0];
        initialSmallRectanglePositions2[1] = smallRectanglePositions2[1];
        initialSmallRectanglePositions2[2] = smallRectanglePositions2[2];
        initialSmallRectanglePositions2[3] = smallRectanglePositions2[3];
        initialSmallRectanglePositions2[4] = smallRectanglePositions2[4];
        initialSmallRectanglePositions2[5] = smallRectanglePositions2[5];
        initialSmallRectanglePositions2[6] = smallRectanglePositions2[6];
        initialSmallRectanglePositions2[7] = smallRectanglePositions2[7];

        // Translate the entire geometry to center
        TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(squarePositions, numSquareVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(rightTrianglePositions1, numRightTriangleVertices1, -scale * 0.4f, 0.0f);
        TranslateToCenter(rightTrianglePositions2, numRightTriangleVertices2, -scale * 0.4f, 0.0f);
        TranslateToCenter(rightTrianglePositions3, numRightTriangleVertices3, -scale * 0.4f, 0.0f);
        TranslateToCenter(rightTrianglePositions4, numRightTriangleVertices4, -scale * 0.4f, 0.0f);
        TranslateToCenter(smallRectanglePositions1, numSmallRectangleVertices1, -scale * 0.4f, 0.0f);
        TranslateToCenter(smallRectanglePositions2, numSmallRectangleVertices2, -scale * 0.4f, 0.0f);

        TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialSquarePositions, numSquareVertices, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRightTrianglePositions1, numRightTriangleVertices1, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRightTrianglePositions2, numRightTriangleVertices2, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRightTrianglePositions3, numRightTriangleVertices3, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialRightTrianglePositions4, numRightTriangleVertices4, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialSmallRectanglePositions1, numSmallRectangleVertices1, -scale * 0.4f, 0.0f);
        TranslateToCenter(initialSmallRectanglePositions2, numSmallRectangleVertices2, -scale * 0.4f, 0.0f);

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

        // Square indices
        squareIndices[0] = 0; // Top right
        squareIndices[1] = 1; // Bottom right
        squareIndices[2] = 2; // Bottom left
        squareIndices[3] = 0; // Top right
        squareIndices[4] = 2; // Bottom left
        squareIndices[5] = 3; // Top left

        // Right triangle1 indices
        rightTriangleIndices1[0] = 0; // Bottom right
        rightTriangleIndices1[1] = 1; // Bottom left
        rightTriangleIndices1[2] = 2; // Top left

        // Right triangle2 indices
        rightTriangleIndices2[0] = 0; // Top right
        rightTriangleIndices2[1] = 1; // Top left
        rightTriangleIndices2[2] = 2; // Bottom left

        // Right triangle3 indices
        rightTriangleIndices3[0] = 0; // Top right
        rightTriangleIndices3[1] = 1; // Bottom right
        rightTriangleIndices3[2] = 2; // Bottom left

        // Right triangle4 indices
        rightTriangleIndices4[0] = 0; // Bottom right
        rightTriangleIndices4[1] = 1; // Bottom left
        rightTriangleIndices4[2] = 2; // Top left

        // Small rectangle1 indices
        smallRectangleIndices1[0] = 0; // Top right
        smallRectangleIndices1[1] = 1; // Bottom right
        smallRectangleIndices1[2] = 2; // Bottom left
        smallRectangleIndices1[3] = 0; // Top right
        smallRectangleIndices1[4] = 2; // Bottom left
        smallRectangleIndices1[5] = 3; // Top left

        // Small rectangle2 indices
        smallRectangleIndices2[0] = 0; // Top right
        smallRectangleIndices2[1] = 1; // Bottom right
        smallRectangleIndices2[2] = 2; // Bottom left
        smallRectangleIndices2[3] = 0; // Top right
        smallRectangleIndices2[4] = 2; // Bottom left
        smallRectangleIndices2[5] = 3; // Top left

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

        // Vertex Array Object and Buffer for square
        GL.GenVertexArrays(1, out squareVao);
        GL.GenBuffers(1, out squareVbo);
        GL.GenBuffers(1, out squareEbo);

        // Initialize VAO and VBO for square
        GL.BindVertexArray(squareVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numSquareVertices * 2, squarePositions, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, squareEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numSquareIndices, squareIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for right triangle1
        GL.GenVertexArrays(1, out rightTriangleVao1);
        GL.GenBuffers(1, out rightTriangleVbo1);
        GL.GenBuffers(1, out rightTriangleEbo1);

        // Initialize VAO and VBO for right triangle1
        GL.BindVertexArray(rightTriangleVao1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo1);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rightTriangleEbo1);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numRightTriangleIndices1, rightTriangleIndices1, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for right triangle2
        GL.GenVertexArrays(1, out rightTriangleVao2);
        GL.GenBuffers(1, out rightTriangleVbo2);
        GL.GenBuffers(1, out rightTriangleEbo2);

        // Initialize VAO and VBO for right triangle2
        GL.BindVertexArray(rightTriangleVao2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rightTriangleEbo2);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numRightTriangleIndices2, rightTriangleIndices2, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for right triangle3
        GL.GenVertexArrays(1, out rightTriangleVao3);
        GL.GenBuffers(1, out rightTriangleVbo3);
        GL.GenBuffers(1, out rightTriangleEbo3);

        // Initialize VAO and VBO for right triangle3
        GL.BindVertexArray(rightTriangleVao3);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo3);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numRightTriangleVertices3 * 2, rightTrianglePositions3, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rightTriangleEbo3);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numRightTriangleIndices3, rightTriangleIndices3, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for right triangle4
        GL.GenVertexArrays(1, out rightTriangleVao4);
        GL.GenBuffers(1, out rightTriangleVbo4);
        GL.GenBuffers(1, out rightTriangleEbo4);

        // Initialize VAO and VBO for right triangle4
        GL.BindVertexArray(rightTriangleVao4);
        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo4);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numRightTriangleVertices4 * 2, rightTrianglePositions4, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, rightTriangleEbo4);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numRightTriangleIndices4, rightTriangleIndices4, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for small rectangle1
        GL.GenVertexArrays(1, out smallRectangleVao1);
        GL.GenBuffers(1, out smallRectangleVbo1);
        GL.GenBuffers(1, out smallRectangleEbo1);

        // Initialize VAO and VBO for small rectangle1
        GL.BindVertexArray(smallRectangleVao1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo1);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numSmallRectangleVertices1 * 2, smallRectanglePositions1, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, smallRectangleEbo1);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numSmallRectangleIndices1, smallRectangleIndices1, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);

        // Vertex Array Object and Buffer for small rectangle2
        GL.GenVertexArrays(1, out smallRectangleVao2);
        GL.GenBuffers(1, out smallRectangleVbo2);
        GL.GenBuffers(1, out smallRectangleEbo2);

        // Initialize VAO and VBO for small rectangle2
        GL.BindVertexArray(smallRectangleVao2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * numSmallRectangleVertices2 * 2, smallRectanglePositions2, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, smallRectangleEbo2);
        GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * numSmallRectangleIndices2, smallRectangleIndices2, BufferUsageHint.StaticDraw);

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

        for (int i = 0; i < numSquareVertices * 2; i += 2)
        {
            float x = initialSquarePositions[i] - centerX;
            float y = initialSquarePositions[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            squarePositions[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            squarePositions[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRightTriangleVertices1 * 2; i += 2)
        {
            float x = initialRightTrianglePositions1[i] - centerX;
            float y = initialRightTrianglePositions1[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rightTrianglePositions1[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            rightTrianglePositions1[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRightTriangleVertices2 * 2; i += 2)
        {
            float x = initialRightTrianglePositions2[i] - centerX;
            float y = initialRightTrianglePositions2[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rightTrianglePositions2[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            rightTrianglePositions2[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRightTriangleVertices3 * 2; i += 2)
        {
            float x = initialRightTrianglePositions3[i] - centerX;
            float y = initialRightTrianglePositions3[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rightTrianglePositions3[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            rightTrianglePositions3[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRightTriangleVertices4 * 2; i += 2)
        {
            float x = initialRightTrianglePositions4[i] - centerX;
            float y = initialRightTrianglePositions4[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rightTrianglePositions4[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            rightTrianglePositions4[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numSmallRectangleVertices1 * 2; i += 2)
        {
            float x = initialSmallRectanglePositions1[i] - centerX;
            float y = initialSmallRectanglePositions1[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            smallRectanglePositions1[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            smallRectanglePositions1[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numSmallRectangleVertices2 * 2; i += 2)
        {
            float x = initialSmallRectanglePositions2[i] - centerX;
            float y = initialSmallRectanglePositions2[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            smallRectanglePositions2[i] = (float)Math.Cos(-angle) * x - (float)Math.Sin(-angle) * y + centerX; // New x
            smallRectanglePositions2[i + 1] = (float)Math.Sin(-angle) * x + (float)Math.Cos(-angle) * y + centerY; // New y
        }

        // Update the vertex buffer with the new positions
        GL.BindBuffer(BufferTarget.ArrayBuffer, circleVbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numCircleVertices * 2, circlePositions);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rectangleVbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRectangleVertices * 2, rectanglePositions);

        GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSquareVertices * 2, squarePositions);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo1);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo2);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo3);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices3 * 2, rightTrianglePositions3);

        GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo4);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices4 * 2, rightTrianglePositions4);

        GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo1);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSmallRectangleVertices1 * 2, smallRectanglePositions1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo2);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSmallRectangleVertices2 * 2, smallRectanglePositions2);
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
        else if (positions == squarePositions)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSquareVertices * 2, squarePositions);
        }
        else if (positions == rightTrianglePositions1)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo1);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1);
        }
        else if (positions == rightTrianglePositions2)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo2);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2);
        }
        else if (positions == rightTrianglePositions3)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo3);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices3 * 2, rightTrianglePositions3);
        }
        else if (positions == rightTrianglePositions4)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, rightTriangleVbo4);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numRightTriangleVertices4 * 2, rightTrianglePositions4);
        }
        else if (positions == smallRectanglePositions1)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo1);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSmallRectangleVertices1 * 2, smallRectanglePositions1);
        }
        else if (positions == smallRectanglePositions2)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, smallRectangleVbo2);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * numSmallRectangleVertices2 * 2, smallRectanglePositions2);
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
        TranslateArbitrary(squarePositions, numSquareVertices, offsetX, offsetY);
        TranslateArbitrary(rightTrianglePositions1, numRightTriangleVertices1, offsetX, offsetY);
        TranslateArbitrary(rightTrianglePositions2, numRightTriangleVertices2, offsetX, offsetY);
        TranslateArbitrary(rightTrianglePositions3, numRightTriangleVertices3, offsetX, offsetY);
        TranslateArbitrary(rightTrianglePositions4, numRightTriangleVertices4, offsetX, offsetY);
        TranslateArbitrary(smallRectanglePositions1, numSmallRectangleVertices1, offsetX, offsetY);
        TranslateArbitrary(smallRectanglePositions2, numSmallRectangleVertices2, offsetX, offsetY);
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

            // Draw the square outline
            GL.BindVertexArray(squareVao);
            GL.DrawElements(PrimitiveType.LineLoop, numSquareIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw small rectangle 1 outline
            GL.BindVertexArray(smallRectangleVao1);
            GL.DrawElements(PrimitiveType.LineLoop, numSmallRectangleIndices1, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw small rectangle 2 outline
            GL.BindVertexArray(smallRectangleVao2);
            GL.DrawElements(PrimitiveType.LineLoop, numSmallRectangleIndices2, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw right triangle 1 outline
            GL.BindVertexArray(rightTriangleVao1);
            GL.DrawElements(PrimitiveType.LineLoop, numRightTriangleIndices1, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw right triangle 2 outline
            GL.BindVertexArray(rightTriangleVao2);
            GL.DrawElements(PrimitiveType.LineLoop, numRightTriangleIndices2, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw right triangle 3 outline
            GL.BindVertexArray(rightTriangleVao3);
            GL.DrawElements(PrimitiveType.LineLoop, numRightTriangleIndices3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw right triangle 4 outline
            GL.BindVertexArray(rightTriangleVao4);
            GL.DrawElements(PrimitiveType.LineLoop, numRightTriangleIndices4, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the fill
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f); // Set color to metallic gray/silver

            // Draw the circles fill
            GL.BindVertexArray(circleVao);
            GL.DrawElements(PrimitiveType.Triangles, numCircleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the rectangle fill
            GL.BindVertexArray(rectangleVao);
            GL.DrawElements(PrimitiveType.Triangles, numRectangleIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the square fill
            GL.BindVertexArray(squareVao);
            GL.DrawElements(PrimitiveType.Triangles, numSquareIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the right triangle1 fill
            GL.BindVertexArray(rightTriangleVao1);
            GL.DrawElements(PrimitiveType.Triangles, numRightTriangleIndices1, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the right triangle2 fill
            GL.BindVertexArray(rightTriangleVao2);
            GL.DrawElements(PrimitiveType.Triangles, numRightTriangleIndices2, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the right triangle3 fill
            GL.BindVertexArray(rightTriangleVao3);
            GL.DrawElements(PrimitiveType.Triangles, numRightTriangleIndices3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the right triangle4 fill
            GL.BindVertexArray(rightTriangleVao4);
            GL.DrawElements(PrimitiveType.Triangles, numRightTriangleIndices4, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the small rectangle1 fill
            GL.BindVertexArray(smallRectangleVao1);
            GL.DrawElements(PrimitiveType.Triangles, numSmallRectangleIndices1, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Draw the small rectangle2 fill
            GL.BindVertexArray(smallRectangleVao2);
            GL.DrawElements(PrimitiveType.Triangles, numSmallRectangleIndices2, DrawElementsType.UnsignedInt, IntPtr.Zero);
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
