using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using PT_Sim;

class HA75Aligner
{
    private float scale;

    private int numHousingRectangleVertices;
    private int numChuckVertices;

    private int numHousingRectangleIndices;
    private int numChuckIndices;

    private int housingRectangleVao, housingRectangleVbo, housingRectangleEbo;
    private int chuckVao, chuckVbo, chuckEbo;

    private float[] housingRectanglePositions;
    private float[] chuckPosiotions;

    private int[] housingRectangleIndices;
    private int[] chuckIndices;

    private const float PI = 3.14159265358979323846f;

    public HA75Aligner(float scale)
    {
        this.scale = scale;

        this.numHousingRectangleVertices = 4;
        this.numChuckVertices = 2 * (20 + 1);
        this.numHousingRectangleIndices = 6;
        this.numChuckIndices = 2 * 3 * 20;

        this.housingRectanglePositions = new float[]
        {
            -0.5f, -0.5f,
            0.5f, -0.5f,
            0.5f, 0.5f,
            -0.5f, 0.5f
        };

        housingRectangleIndices = new int[]
        {
           0, 1, 2,
           2, 3, 0
        };
    }

    ~HA75Aligner()
    {
        GL.DeleteVertexArray(housingRectangleVao);
        GL.DeleteBuffer(housingRectangleVbo);
        GL.DeleteBuffer(housingRectangleEbo);
    }

    public void Initialize()
    {
        GL.GenVertexArrays(1, out housingRectangleVao);
        GL.GenBuffers(1, out housingRectangleVbo);
        GL.GenBuffers(1, out housingRectangleEbo);
        if (housingRectangleVao == 0 || housingRectangleVbo == 0 || housingRectangleEbo == 0)
        {
            Logger.Log("Error:", "VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Setup VAO
        GL.BindVertexArray(housingRectangleVao);

        // Upload vertex data
        GL.BindBuffer(BufferTarget.ArrayBuffer, housingRectangleVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, housingRectanglePositions.Length * sizeof(float), housingRectanglePositions, BufferUsageHint.StaticDraw);

        // Upload index data
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, housingRectangleEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, housingRectangleIndices.Length * sizeof(int), housingRectangleIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO
    }

    public void Render(Shader shader)
    {
        shader.Bind();
        int location = shader.GetUniformLocation("u_Color");

        if (location != -1)
        {
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(housingRectangleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numHousingRectangleIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numHousingRectangleIndices, DrawElementsType.UnsignedInt, 0);
            
            GL.BindVertexArray(0);
        }
    }
}
