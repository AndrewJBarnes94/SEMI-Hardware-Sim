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
    private float[] chuckPositions;

    private int[] housingRectangleIndices;
    private uint[] chuckIndices;

    private const float PI = 3.14159265358979323846f;

    private float posAx, posAy;
    private float posBx, posBy;
    private float posCx, posCy;
    private float posDx, posDy;

    private float chuckCenterX, chuckCenterY;
    private float chuckRadius;

    public HA75Aligner(
        float scale,
        float posAx,
        float posAy,
        float posBx,
        float posBy,
        float posCx,
        float posCy,
        float posDx,
        float posDy,
        float chuckCenterX,
        float chuckCenterY,
        float chuckRadius
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

        this.chuckCenterX = chuckCenterX;
        this.chuckCenterY = chuckCenterY;
        this.chuckRadius = chuckRadius;

        this.numHousingRectangleVertices = 4;
        this.numChuckVertices = 21; // 20 segments + center

        this.numHousingRectangleIndices = 6;
        this.numChuckIndices = 3 * 20; // 20 triangles

        this.housingRectanglePositions = new float[]
        {
            posAx * scale, posAy * scale,
            posBx * scale, posBy * scale,
            posCx * scale, posCy * scale,
            posDx * scale, posDy * scale
        };

        housingRectangleIndices = new int[]
        {
           0, 1, 2,
           1, 3, 2
        };

        // Initialize chuck positions and indices
        chuckPositions = new float[numChuckVertices * 2];
        chuckIndices = new uint[numChuckIndices];

        // Center of the chuck
        chuckPositions[0] = chuckCenterX * scale;
        chuckPositions[1] = chuckCenterY * scale;

        // Chuck vertices
        for (int i = 1; i <= 20; ++i)
        {
            float theta = 2.0f * PI * i / 20;
            chuckPositions[2 * i] = chuckCenterX * scale + chuckRadius * scale * (float)Math.Cos(theta); // x
            chuckPositions[2 * i + 1] = chuckCenterY * scale + chuckRadius * scale * (float)Math.Sin(theta); // y
        }

        // Chuck indices
        int index = 0;
        for (int i = 1; i <= 20; ++i)
        {
            chuckIndices[index++] = 0;
            chuckIndices[index++] = (uint)i;
            chuckIndices[index++] = (uint)(i % 20 + 1);
        }
    }

    ~HA75Aligner()
    {
        GL.DeleteVertexArray(housingRectangleVao);
        GL.DeleteBuffer(housingRectangleVbo);
        GL.DeleteBuffer(housingRectangleEbo);

        GL.DeleteVertexArray(chuckVao);
        GL.DeleteBuffer(chuckVbo);
        GL.DeleteBuffer(chuckEbo);
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

        // Initialize chuck VAO, VBO, and EBO
        GL.GenVertexArrays(1, out chuckVao);
        GL.GenBuffers(1, out chuckVbo);
        GL.GenBuffers(1, out chuckEbo);

        GL.BindVertexArray(chuckVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, chuckVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, chuckPositions.Length * sizeof(float), chuckPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, chuckEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, chuckIndices.Length * sizeof(uint), chuckIndices, BufferUsageHint.StaticDraw);

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

            // Render the chuck
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(chuckVao);
            GL.DrawElements(PrimitiveType.LineLoop, numChuckIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numChuckIndices, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }

        shader.Unbind();
    }

    public List<float> GetPositionMap(string point)
    {
        Dictionary<string, List<float>> positionMap = new Dictionary<string, List<float>>
        {
            { "topLeft", new List<float>
                {
                    housingRectanglePositions[0], housingRectanglePositions[1]
                }
            },
            { "topRight", new List<float>
                {
                    housingRectanglePositions[2], housingRectanglePositions[3]
                }
            },
            { "bottomRight", new List<float>
                {
                    housingRectanglePositions[4], housingRectanglePositions[5]
                }
            },
            { "bottomLeft", new List<float>
                {
                    housingRectanglePositions[6], housingRectanglePositions[7]
                }
            }
        };

        return positionMap[point];
    }
}
