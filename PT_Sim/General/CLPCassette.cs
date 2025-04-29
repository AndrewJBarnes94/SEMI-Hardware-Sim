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

    private int numUpperRecVertices;
    private int numUpperRecIndices;
    private int upperRecVao, upperRecVbo, upperRecEbo;
    private float[] upperRecPositions;
    private uint[] upperRecIndices;
    float x9, y9, x10, y10, x11, y11, x12, y12;

    private int numUpperTri1Vertices;
    private int numUpperTri1Indices;
    private int upperTri1Vao, upperTri1Vbo, upperTri1Ebo;
    private float[] upperTri1Positions;
    private uint[] upperTri1Indices;
    float x13, y13, x14, y14, x15, y15;

    private int numUpperTri2Vertices;
    private int numUpperTri2Indices;
    private int upperTri2Vao, upperTri2Vbo, upperTri2Ebo;
    private float[] upperTri2Positions;
    private uint[] upperTri2Indices;
    float x16, y16, x17, y17, x18, y18;

    private int numUpperSmallRec1Vertices;
    private int numUpperSmallRec1Indices;
    private int upperSmallRec1Vao, upperSmallRec1Vbo, upperSmallRec1Ebo;
    private float[] upperSmallRec1Positions;
    private uint[] upperSmallRec1Indices;
    float x19, y19, x20, y20, x21, y21, x22, y22;

    private int numUpperSmallRec2Vertices;
    private int numUpperSmallRec2Indices;
    private int upperSmallRec2Vao, upperSmallRec2Vbo, upperSmallRec2Ebo;
    private float[] upperSmallRec2Positions;
    private uint[] upperSmallRec2Indices;
    float x23, y23, x24, y24, x25, y25, x26, y26;

    private int numIrregularTrap1Vertices;
    private int numIrregularTrap1Indices;
    private int irregularTrap1Vao, irregularTrap1Vbo, irregularTrap1Ebo;
    private float[] irregularTrap1Positions;
    private uint[] irregularTrap1Indices;
    float x27, y27, x28, y28, x29, y29, x30, y30;

    private int numIrregularTrap2Vertices;
    private int numIrregularTrap2Indices;
    private int irregularTrap2Vao, irregularTrap2Vbo, irregularTrap2Ebo;
    private float[] irregularTrap2Positions;
    private uint[] irregularTrap2Indices;
    float x31, y31, x32, y32, x33, y33, x34, y34;

    public CLPCassette(
        float scale,
        float x1, float y1,
        float x2, float y2,
        float x3, float y3,
        float x4, float y4,

        float x5, float y5,
        float x6, float y6,
        float x7, float y7,
        float x8, float y8,

        float x9, float y9,
        float x10, float y10,
        float x11, float y11,
        float x12, float y12,

        float x13, float y13,
        float x14, float y14,
        float x15, float y15,

        float x16, float y16,
        float x17, float y17,
        float x18, float y18,

        float x19, float y19,
        float x20, float y20,
        float x21, float y21,
        float x22, float y22,

        float x23, float y23,
        float x24, float y24,
        float x25, float y25,
        float x26, float y26,

        float x27, float y27,
        float x28, float y28,
        float x29, float y29,
        float x30, float y30,

        float x31, float y31,
        float x32, float y32,
        float x33, float y33,
        float x34, float y34
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

        numUpperRecVertices = 4;
        numUpperRecIndices = 6;

        this.x9 = x9; this.y9 = y9;
        this.x10 = x10; this.y10 = y10;
        this.x11 = x11; this.y11 = y11;
        this.x12 = x12; this.y12 = y12;

        numUpperTri1Vertices = 3;
        numUpperTri1Indices = 3;

        this.x13 = x13; this.y13 = y13;
        this.x14 = x14; this.y14 = y14;
        this.x15 = x15; this.y15 = y15;

        numUpperTri2Vertices = 3;
        numUpperTri2Indices = 3;

        this.x16 = x16; this.y16 = y16;
        this.x17 = x17; this.y17 = y17;
        this.x18 = x18; this.y18 = y18;

        numUpperSmallRec1Vertices = 4;
        numUpperSmallRec1Indices = 6;

        this.x19 = x19; this.y19 = y19;
        this.x20 = x20; this.y20 = y20;
        this.x21 = x21; this.y21 = y21;
        this.x22 = x22; this.y22 = y22;

        numUpperSmallRec2Vertices = 4;
        numUpperSmallRec2Indices = 6;

        this.x23 = x23; this.y23 = y23;
        this.x24 = x24; this.y24 = y24;
        this.x25 = x25; this.y25 = y25;
        this.x26 = x26; this.y26 = y26;

        numIrregularTrap1Vertices = 4;
        numIrregularTrap1Indices = 6;

        this.x27 = x27; this.y27 = y27;
        this.x28 = x28; this.y28 = y28;
        this.x29 = x29; this.y29 = y29;
        this.x30 = x30; this.y30 = y30;

        numIrregularTrap2Vertices = 4;
        numIrregularTrap2Indices = 6;

        this.x31 = x31; this.y31 = y31;
        this.x32 = x32; this.y32 = y32;
        this.x33 = x33; this.y33 = y33;
        this.x34 = x34; this.y34 = y34;
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

        recIndices = new uint[] { 0, 1, 2, 3, 1, 2 };

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
            scale * x7, scale * y7,
            scale * x3, scale * y3,
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
            scale * x6, scale * y6,
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

        // Upper Rectangle initialization
        upperRecPositions = new float[]
        {
            scale * x9, scale * y9,
            scale * x10, scale * y10,
            scale * x11, scale * y11,
            scale * x12, scale * y12,
        };
        
        upperRecIndices = new uint[] { 0, 1, 2, 3, 1, 2 };

        GL.GenVertexArrays(1, out upperRecVao);
        GL.GenBuffers(1, out upperRecVbo);
        GL.GenBuffers(1, out upperRecEbo);
        if (upperRecVao == 0 || upperRecVbo == 0 || upperRecEbo == 0)
        {
            Logger.Log("Error:", "Upper Rectangle VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(upperRecVao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, upperRecVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, upperRecPositions.Length * sizeof(float), upperRecPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, upperRecEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, upperRecIndices.Length * sizeof(uint), upperRecIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Upper Triangle 1 initialization
        upperTri1Positions = new float[]
        {
            scale * x13, scale * y13,
            scale * x14, scale * y14,
            scale * x15, scale * y15,
        };

        upperTri1Indices = new uint[] { 0, 1, 2 };

        GL.GenVertexArrays(1, out upperTri1Vao);
        GL.GenBuffers(1, out upperTri1Vbo);
        GL.GenBuffers(1, out upperTri1Ebo);
        if (upperTri1Vao == 0 || upperTri1Vbo == 0 || upperTri1Ebo == 0)
        {
            Logger.Log("Error:", "Upper Triangle 1 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(upperTri1Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, upperTri1Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, upperTri1Positions.Length * sizeof(float), upperTri1Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, upperTri1Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, upperTri1Indices.Length * sizeof(uint), upperTri1Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Upper Triangle 2 initialization
        upperTri2Positions = new float[]
        {
            scale * x16, scale * y16,
            scale * x17, scale * y17,
            scale * x18, scale * y18,
        };

        upperTri2Indices = new uint[] { 0, 1, 2 };

        GL.GenVertexArrays(1, out upperTri2Vao);
        GL.GenBuffers(1, out upperTri2Vbo);
        GL.GenBuffers(1, out upperTri2Ebo);
        if (upperTri2Vao == 0 || upperTri2Vbo == 0 || upperTri2Ebo == 0)
        {
            Logger.Log("Error:", "Upper Triangle 2 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(upperTri2Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, upperTri2Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, upperTri2Positions.Length * sizeof(float), upperTri2Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, upperTri2Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, upperTri2Indices.Length * sizeof(uint), upperTri2Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Upper Small Rectangle 1 Initialization
        upperSmallRec1Positions = new float[]
        {
            scale * x19, scale * y19,
            scale * x20, scale * y20,
            scale * x21, scale * y21,
            scale * x22, scale * y22
        };

        upperSmallRec1Indices = new uint[] { 0, 1, 2, 3, 1, 2 };

        GL.GenVertexArrays(1, out upperSmallRec1Vao);
        GL.GenBuffers(1, out upperSmallRec1Vbo);
        GL.GenBuffers(1, out upperSmallRec1Ebo);
        if (upperSmallRec1Vao == 0 || upperSmallRec1Vbo == 0 || upperSmallRec1Ebo == 0)
        {
            Logger.Log("Error:", "Upper Small Rectangle 1 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(upperSmallRec1Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, upperSmallRec1Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, upperSmallRec1Positions.Length * sizeof(float), upperSmallRec1Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, upperSmallRec1Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, upperSmallRec1Indices.Length * sizeof(uint), upperSmallRec1Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Upper Small Rectangle 2 Initialization
        upperSmallRec2Positions = new float[]
        {
            scale * x23, scale * y23,
            scale * x24, scale * y24,
            scale * x25, scale * y25,
            scale * x26, scale * y26
        };

        upperSmallRec2Indices = new uint[] { 0, 1, 2, 3, 1, 2 };

        GL.GenVertexArrays(1, out upperSmallRec2Vao);
        GL.GenBuffers(1, out upperSmallRec2Vbo);
        GL.GenBuffers(1, out upperSmallRec2Ebo);
        if (upperSmallRec2Vao == 0 || upperSmallRec2Vbo == 0 || upperSmallRec2Ebo == 0)
        {
            Logger.Log("Error:", "Upper Small Rectangle 2 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(upperSmallRec2Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, upperSmallRec2Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, upperSmallRec2Positions.Length * sizeof(float), upperSmallRec2Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, upperSmallRec2Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, upperSmallRec2Indices.Length * sizeof(uint), upperSmallRec2Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Irregular Triangle 1 Initialization
        irregularTrap1Positions = new float[]
        {
            scale * x27, scale * y27,
            scale * x28, scale * y28,
            scale * x29, scale * y29,
            scale * x30, scale * y30
        };

        irregularTrap1Indices = new uint[] { 0, 1, 2, 3, 1, 2 };

        GL.GenVertexArrays(1, out irregularTrap1Vao);
        GL.GenBuffers(1, out irregularTrap1Vbo);
        GL.GenBuffers(1, out irregularTrap1Ebo);
        if (irregularTrap1Vao == 0 || irregularTrap1Vbo == 0 || irregularTrap1Ebo == 0)
        {
            Logger.Log("Error:", "Irregular Trapezoid 1 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(irregularTrap1Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, irregularTrap1Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, irregularTrap1Positions.Length * sizeof(float), irregularTrap1Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, irregularTrap1Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, irregularTrap1Indices.Length * sizeof(uint), irregularTrap1Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Irregular Triangle 2 Initialization
        irregularTrap2Positions = new float[]
        {
            scale * x31, scale * y31,
            scale * x32, scale * y32,
            scale * x33, scale * y33,
            scale * x34, scale * y34
        };

        irregularTrap2Indices = new uint[] { 0, 1, 2, 3, 1, 2 };

        GL.GenVertexArrays(1, out irregularTrap2Vao);
        GL.GenBuffers(1, out irregularTrap2Vbo);
        GL.GenBuffers(1, out irregularTrap2Ebo);
        if (irregularTrap2Vao == 0 || irregularTrap2Vbo == 0 || irregularTrap2Ebo == 0)
        {
            Logger.Log("Error:", "Irregular Trapezoid 2 VAO, VBO, or EBO not initialized correctly");
            return;
        }

        GL.BindVertexArray(irregularTrap2Vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, irregularTrap2Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, irregularTrap2Positions.Length * sizeof(float), irregularTrap2Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, irregularTrap2Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, irregularTrap2Indices.Length * sizeof(uint), irregularTrap2Indices, BufferUsageHint.StaticDraw);

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

            // Set outline color (black)
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            // Draw outlines for all shapes
            GL.BindVertexArray(recVao);
            GL.DrawElements(PrimitiveType.LineLoop, numRecIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(squareVao);
            GL.DrawElements(PrimitiveType.LineLoop, squareIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(triLVao);
            GL.DrawElements(PrimitiveType.LineLoop, numTriLIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(triRVao);
            GL.DrawElements(PrimitiveType.LineLoop, numTriRIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Set fill color (same as rectangle: #19605F)
            GL.Uniform4(location, 0.095f, 0.35f, 0.35f, 1.0f);

            // Draw filled interiors for all shapes
            GL.BindVertexArray(recVao);
            GL.DrawElements(PrimitiveType.Triangles, numRecIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(squareVao);
            GL.DrawElements(PrimitiveType.Triangles, squareIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(triLVao);
            GL.DrawElements(PrimitiveType.Triangles, numTriLIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(triRVao);
            GL.DrawElements(PrimitiveType.Triangles, numTriRIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);


            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            GL.BindVertexArray(upperRecVao);
            GL.DrawElements(PrimitiveType.LineLoop, upperRecIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperTri1Vao);
            GL.DrawElements(PrimitiveType.LineLoop, upperTri1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperTri2Vao);
            GL.DrawElements(PrimitiveType.LineLoop, upperTri2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperSmallRec1Vao);
            GL.DrawElements(PrimitiveType.LineLoop, upperSmallRec1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperSmallRec2Vao);
            GL.DrawElements(PrimitiveType.LineLoop, upperSmallRec2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(irregularTrap1Vao);
            GL.DrawElements(PrimitiveType.LineLoop, irregularTrap1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(irregularTrap2Vao);
            GL.DrawElements(PrimitiveType.LineLoop, irregularTrap2Indices.Length, DrawElementsType.UnsignedInt, 0);

            // Set fill color
            GL.Uniform4(location, 0.105f, 0.431f, 0.430f, 1.0f);

            // Fill the shape
            GL.BindVertexArray(upperRecVao);
            GL.DrawElements(PrimitiveType.Triangles, upperRecIndices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperTri1Vao);
            GL.DrawElements(PrimitiveType.Triangles, upperTri1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            GL.BindVertexArray(upperTri2Vao);
            GL.DrawElements(PrimitiveType.Triangles, upperTri2Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperSmallRec1Vao);
            GL.DrawElements(PrimitiveType.Triangles, upperSmallRec1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(upperSmallRec2Vao);
            GL.DrawElements(PrimitiveType.Triangles, upperSmallRec2Indices.Length, DrawElementsType.UnsignedInt, 0);            
            GL.BindVertexArray(irregularTrap1Vao);
            GL.DrawElements(PrimitiveType.Triangles, irregularTrap1Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(irregularTrap2Vao);
            GL.DrawElements(PrimitiveType.Triangles, irregularTrap2Indices.Length, DrawElementsType.UnsignedInt, 0);
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

        // Dispose upper rectangle resources
        if (upperRecVao != 0) GL.DeleteVertexArray(upperRecVao);
        if (upperRecVbo != 0) GL.DeleteBuffer(upperRecVbo);
        if (upperRecEbo != 0) GL.DeleteBuffer(upperRecEbo);

        upperRecVao = upperRecVbo = upperRecEbo = 0;

        // Dispose upper triangle 1 resources
        if (upperTri1Vao != 0) GL.DeleteVertexArray(upperTri1Vao);
        if (upperTri1Vbo != 0) GL.DeleteBuffer(upperTri1Vbo);
        if (upperTri1Ebo != 0) GL.DeleteBuffer(upperTri1Ebo);

        upperTri1Vao = upperTri1Vbo = upperTri1Ebo = 0;

        // Dispose upper triangle 2 resources
        if (upperTri2Vao != 0) GL.DeleteVertexArray(upperTri2Vao);
        if (upperTri2Vbo != 0) GL.DeleteBuffer(upperTri2Vbo);
        if (upperTri2Ebo != 0) GL.DeleteBuffer(upperTri2Ebo);

        upperTri2Vao = upperTri2Vbo = upperTri2Ebo = 0;

        // Dispose upper small rectangle 1 resources
        if (upperSmallRec1Vao != 0) GL.DeleteVertexArray(upperSmallRec1Vao);
        if (upperSmallRec1Vbo != 0) GL.DeleteBuffer(upperSmallRec1Vbo);
        if (upperSmallRec1Ebo != 0) GL.DeleteBuffer(upperSmallRec1Ebo);

        upperSmallRec1Vao = upperSmallRec1Vbo = upperSmallRec1Ebo = 0;

        // Dispose upper small rectangle 2 resources
        if (upperSmallRec2Vao != 0) GL.DeleteVertexArray(upperSmallRec2Vao);
        if (upperSmallRec2Vbo != 0) GL.DeleteBuffer(upperSmallRec2Vbo);
        if (upperSmallRec2Ebo != 0) GL.DeleteBuffer(upperSmallRec2Ebo);

        upperSmallRec2Vao = upperSmallRec2Vbo = upperSmallRec2Ebo = 0;

        GC.SuppressFinalize(this);
    }
}