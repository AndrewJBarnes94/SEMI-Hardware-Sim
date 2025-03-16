using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using PT_Sim;


public class ProcessModule
{
    private float _scale;
    
    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;

    private int vao, vbo, ebo;

    private float posAx, posAy;
    private float posBx, posBy;
    private float posCx, posCy;
    private float posDx, posDy;

    // Wafer Platform
    private (float, float) waferPlatformCenter;
    private float waferPlatformRadius;

    private int waferPlatformNumCircVertices;
    private int waferPlatformNumVertices;
    private int waferPlatformNumIndices;
    private float[] waferPlatformPositions;
    private uint[] waferPlatformIndices;

    private int waferPlatformVao, waferPlatformVbo, waferPlatformEbo;


    public ProcessModule(
        float scale,

        float posAx,
        float posAy,
        float posBx,
        float posBy,
        float posCx,
        float posCy,
        float posDx,
        float posDy,

        (float, float) waferPlatformCenter,
        float waferPlatformRadius
    )
    {
        this._scale = scale;

        // PM Chamber
        numVertices = 4;
        numIndices = 6;
        positions = new float[]
        {
            posAx * scale, posAy * scale,
            posBx * scale, posBy * scale,
            posCx * scale, posCy * scale,
            posDx * scale, posDy * scale
        };
        indices = new uint[]
        {
            0, 1, 2,
            1, 3, 2
        };

        // PM Wafer Platform
        this.waferPlatformCenter = waferPlatformCenter;
        this.waferPlatformRadius = waferPlatformRadius;

        waferPlatformNumCircVertices = 60;
        waferPlatformNumVertices = waferPlatformNumCircVertices + 1; // Center vertex
        waferPlatformNumIndices = waferPlatformNumVertices * 3;

        waferPlatformPositions = new float[waferPlatformNumVertices * 2];
        waferPlatformIndices = new uint[waferPlatformNumIndices];
    }

    ~ProcessModule()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);

        GL.DeleteVertexArray(waferPlatformVao);
        GL.DeleteBuffer(waferPlatformVbo);
        GL.DeleteBuffer(waferPlatformEbo);
    }

    public void Initialize()
    {
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);
        if (vao == 0 || vbo == 0 || ebo == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(vao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, positions.Length * sizeof(float), positions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Setup vertex attributes
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);


        // PM Wafer Platform
        int index = 0;

        // Center Vertex
        waferPlatformPositions[index++] = waferPlatformCenter.Item1; // X
        waferPlatformPositions[index++] = waferPlatformCenter.Item2; // Y

        // Circle Vertices
        for (int i = 0; i < waferPlatformNumCircVertices; i++)
        {
            float angle = (float)(2.0 * System.Math.PI * i / waferPlatformNumCircVertices);
            waferPlatformPositions[index++] = waferPlatformCenter.Item1 + waferPlatformRadius * (float)System.Math.Cos(angle); // X
            waferPlatformPositions[index++] = waferPlatformCenter.Item2 + waferPlatformRadius * (float)System.Math.Sin(angle); // Y
        }

        index = 0;

        // Define Indices (Triangles forming a fan shape)
        for (int i = 0; i < waferPlatformNumCircVertices; i++)
        {
            waferPlatformIndices[index++] = 0; // Center vertex
            waferPlatformIndices[index++] = (uint)(i + 1);
            waferPlatformIndices[index++] = (uint)((i + 1) % waferPlatformNumCircVertices + 1); // Loop around
        }

        // Generate OpenGL Buffers
        GL.GenVertexArrays(1, out waferPlatformVao);
        GL.GenBuffers(1, out waferPlatformVbo);
        GL.GenBuffers(1, out waferPlatformEbo);
        if (waferPlatformVao == 0 || waferPlatformVbo == 0 || waferPlatformEbo == 0)
        {
            Logger.Log("Error:", "PM Wafer Platform VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(waferPlatformVao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, waferPlatformVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, waferPlatformPositions.Length * sizeof(float), waferPlatformPositions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, waferPlatformEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, waferPlatformIndices.Length * sizeof(uint), waferPlatformIndices, BufferUsageHint.StaticDraw);

        // Define vertex attributes
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

            // Draw Outline using Line Loop (No extra indices needed!)
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw Filled Interior (Gray)
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);

            // PM Wafer Platform
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(waferPlatformVao);
            GL.DrawElements(PrimitiveType.LineLoop, waferPlatformNumIndices, DrawElementsType.UnsignedInt, 0);

            // Draw Filled Interior (Gray)
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, waferPlatformNumIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }

    public List<float> GetPositionMap(string point)
    {
        Dictionary<string, List<float>> positionMap = new Dictionary<string, List<float>>
        {
            { "A", new List<float> { posAx, posAy } },
            { "B", new List<float> { posBx, posBy } },
            { "C", new List<float> { posCx, posCy } },
            { "D", new List<float> { posDx, posDy } },
            { "WaferPlatformCenter", new List<float> { waferPlatformCenter.Item1, waferPlatformCenter.Item2 } },
            { "WaferPlatformRadius", new List<float> { waferPlatformRadius } }
        };

        return positionMap[point];
    }

}

