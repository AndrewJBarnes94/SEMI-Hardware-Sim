using System;
using System.Drawing.Printing;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
public class CLPCassette : IDisposable
{
    private float scale;

    private int numRecVertices;
    private int numRecIndices;
    private int recVao, recVbo, recEbo;
    private float[] recPositions;
    private uint[] recIndices;
    float x1, y1, x2, y2, x3, y3, x4, y4;

    private int numSquareVertices;
    private int numSquareIndices;
    private int squareVao, squareVbo, squareEbo;
    private float[] squarePositions;
    private uint[] squareIndices;
    float x5, y5, x6, y6, x7, y7, x8, y8;

    private int numTriLVertices;
    private int numTriLIndices;
    private int triLVao, triLVbo, triLEbo;
    private float[] triLPositions;
    private uint[] triLIndices;

    private int numTriRVertices;
    private int numTriRIndices;
    private int triRVao, triRVbo, triREbo;
    private float[] triRPositions;
    private uint[] triRIndices;

    public CLPCassette(
        float scale,
        float x1, float y1,
        float x2, float y2,
        float x3, float y3,
        float x4, float y4,

        float x5, float y5,
        float x6, float y6,
        float x7, float y7,
        float x8, float y8
        )
    {
        this.scale = scale;

        this.x1 = x1; this.y1 = y1;
        this.x2 = x2; this.y2 = y2;
        this.x3 = x3; this.y3 = y3;
        this.x4 = x4; this.y4 = y4;

        numRecVertices = 4;
        numRecIndices = 6;

        this.x5 = x5; this.y5 = y5;
        this.x6 = x6; this.y6 = y6;
        this.x7 = x7; this.y7 = y7;
        this.x8 = x8; this.y8 = y8;

        numSquareVertices = 4;
        numSquareIndices = 6;

        numTriLVertices = 3;
        numTriLIndices = 3;

        numTriRVertices = 3;
        numTriRIndices = 3;
    }

    ~CLPCassette()
    {
        Dispose();
    }

    public void Initialize()
    {
        // Rectangle initialization
        recPositions = new float[]
        {
        scale * x1, scale * y1,
        scale * x2, scale * y2,
        scale * x3, scale * y3,
        scale * x4, scale * y4,
        };

        recIndices = new uint[] { 0, 1, 2, 1, 2, 3 };

        GL.GenVertexArrays(1, out recVao);
        GL.GenBuffers(1, out recVbo);
        GL.GenBuffers(1, out recEbo);
        if (recVao == 0 || recVbo == 0 || recEbo == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(recVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, recVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, recPositions.Length * sizeof(float), recPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, recEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, recIndices.Length * sizeof(uint), recIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Square initialization
        squarePositions = new float[]
        {
        scale * x5, scale * y5,
        scale * x6, scale * y6,
        scale * x7, scale * y7,
        scale * x8, scale * y8,
        };

        squareIndices = new uint[] { 0, 1, 2, 1, 2, 3 };

        GL.GenVertexArrays(1, out squareVao);
        GL.GenBuffers(1, out squareVbo);
        GL.GenBuffers(1, out squareEbo);
        if (squareVao == 0 || squareVbo == 0 || squareEbo == 0)
        {
            Logger.Log("Error:", "Square VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(squareVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, squareVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, squarePositions.Length * sizeof(float), squarePositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, squareEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, squareIndices.Length * sizeof(uint), squareIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Triangle initialization (left)
        triLPositions = new float[]
        {
            scale * x5, scale * y5,
            scale * x6, scale * y6,
            scale * x2, scale * y2,
        };

        triLIndices = new uint[] { 0, 1, 2 };

        GL.GenVertexArrays(1, out triLVao);
        GL.GenBuffers(1, out triLVbo);
        GL.GenBuffers(1, out triLEbo);
        if (triLVao == 0 || triLVbo == 0 || triLEbo == 0)
        {
            Logger.Log("Error:", "Triangle left VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(triLVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, triLVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, triLPositions.Length * sizeof(float), triLPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, triLEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, triLIndices.Length * sizeof(uint), triLIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Triangle initialization (right)
        triRPositions = new float[]
        {
            scale * x7, scale * y7,
            scale * x8, scale * y8,
            scale * x4, scale * y4,
        };

        triRIndices = new uint[] { 0, 1, 2 };

        GL.GenVertexArrays(1, out triRVao);
        GL.GenBuffers(1, out triRVbo);
        GL.GenBuffers(1, out triREbo);
        if (triRVao == 0 || triRVbo == 0 || triREbo == 0)
        {
            Logger.Log("Error:", "Triangle right VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(triRVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, triRVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, triRPositions.Length * sizeof(float), triRPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, triREbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, triRIndices.Length * sizeof(uint), triRIndices, BufferUsageHint.StaticDraw);

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

            // Render rectangle (existing code)
            GL.BindVertexArray(recVao);

            // Draw outline
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.LineLoop, numRecIndices, DrawElementsType.UnsignedInt, 0);

            // Draw filled interior
            GL.Uniform4(location, 0.098f, 0.361f, 0.380f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numRecIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            // Render square
            GL.BindVertexArray(squareVao);

            // Draw outline
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.LineLoop, squareIndices.Length, DrawElementsType.UnsignedInt, 0);

            // Draw filled interior
            GL.Uniform4(location, 0.5f, 0.2f, 0.8f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, squareIndices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            // Render triangle left
            GL.BindVertexArray(triLVao);
            // Draw outline
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.LineLoop, numTriLIndices, DrawElementsType.UnsignedInt, 0);
            // Draw filled interior
            GL.Uniform4(location, 0.5f, 0.2f, 0.8f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numTriLIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Render triangle right
            GL.BindVertexArray(triRVao);
            // Draw outline
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.DrawElements(PrimitiveType.LineLoop, numTriRIndices, DrawElementsType.UnsignedInt, 0);
            // Draw filled interior
            GL.Uniform4(location, 0.5f, 0.2f, 0.8f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numTriRIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);


        }

        shader.Unbind();
    }

    public void Dispose()
    {
        // Dispose rectangle resources (existing code)
        if (recVao != 0) GL.DeleteVertexArray(recVao);
        if (recVbo != 0) GL.DeleteBuffer(recVbo);
        if (recEbo != 0) GL.DeleteBuffer(recEbo);

        recVao = recVbo = recEbo = 0;

        // Dispose square resources
        if (squareVao != 0) GL.DeleteVertexArray(squareVao);
        if (squareVbo != 0) GL.DeleteBuffer(squareVbo);
        if (squareEbo != 0) GL.DeleteBuffer(squareEbo);

        squareVao = squareVbo = squareEbo = 0;

        // Dispose triangle left resources
        if (triLVao != 0) GL.DeleteVertexArray(triLVao);
        if (triLVbo != 0) GL.DeleteBuffer(triLVbo);
        if (triLEbo != 0) GL.DeleteBuffer(triLEbo);

        triLVao = triLVbo = triLEbo = 0;

        // Dispose triangle right resources
        if (triRVao != 0) GL.DeleteVertexArray(triRVao);
        if (triRVbo != 0) GL.DeleteBuffer(triRVbo);
        if (triREbo != 0) GL.DeleteBuffer(triREbo);

        triRVao = triRVbo = triREbo = 0;

        GC.SuppressFinalize(this);
    }
}