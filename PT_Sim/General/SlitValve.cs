using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using PT_Sim;

public class SlitValve
{
    private float scale;

    private int numVertices;
    private int numIndices;
    private float[] positions;
    private uint[] indices;

    private int vao, vbo, ebo;

    private float posAx, posAy;
    private float posBx, posBy;
    private float posCx, posCy;
    private float posDx, posDy;

    public SlitValve(
        float scale,
        float posAx,
        float posAy,
        float posBx,
        float posBy,
        float posCx,
        float posCy,
        float posDx,
        float posDy
    )
    {
        this.scale = scale;

        this.posAx = posAx;
        this.posAy = posAy;
        this.posBx = posBx;
        this.posBy = posBy;
        this.posCx = posCx;
        this.posCy = posCy;
        this.posDx = posDx;
        this.posDy = posDy;

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
    }

    ~SlitValve()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
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
            { "D", new List<float> { posDx, posDy } }
        };

        return positionMap[point];
    }
}
