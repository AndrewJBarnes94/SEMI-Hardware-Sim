using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;

class HA75Aligner
{
    public float Scale { get; }
    public float PosAx { get; }
    public float PosAy { get; }
    public float PosBx { get; }
    public float PosBy { get; }
    public float PosCx { get; }
    public float PosCy { get; }
    public float PosDx { get; }
    public float PosDy { get; }
    public float ChuckCenterX { get; }
    public float ChuckCenterY { get; }
    public float ChuckRadius { get; }
    public float HalfCircle1StartX { get; }
    public float HalfCircle1StartY { get; }
    public float HalfCircle1EndX { get; }
    public float HalfCircle1EndY { get; }
    public float HalfCircle2StartX { get; }
    public float HalfCircle2StartY { get; }
    public float HalfCircle2EndX { get; }
    public float HalfCircle2EndY { get; }
    public float OuterSensorAx { get; }
    public float OuterSensorAy { get; }
    public float OuterSensorBx { get; }
    public float OuterSensorBy { get; }
    public float OuterSensorCx { get; }
    public float OuterSensorCy { get; }
    public float OuterSensorDx { get; }
    public float OuterSensorDy { get; }
    public float InnerSensorAx { get; }
    public float InnerSensorAy { get; }
    public float InnerSensorBx { get; }
    public float InnerSensorBy { get; }
    public float InnerSensorCx { get; }
    public float InnerSensorCy { get; }
    public float InnerSensorDx { get; }
    public float InnerSensorDy { get; }

    private int numHousingRectangleVertices;
    private int numChuckVertices;
    private int numHalfCircleVertices;
    private int numOuterSensorVertices;
    private int numInnerSensorVertices;

    private int numHousingRectangleIndices;
    private int numChuckIndices;
    private int numHalfCircleIndices;
    private int numOuterSensorIndices;
    private int numInnerSensorIndices;

    private int housingRectangleVao, housingRectangleVbo, housingRectangleEbo;
    private int chuckVao, chuckVbo, chuckEbo;
    private int halfCircle1Vao, halfCircle1Vbo, halfCircle1Ebo;
    private int halfCircle2Vao, halfCircle2Vbo, halfCircle2Ebo;
    private int outerSensorVao, outerSensorVbo, outerSensorEbo;
    private int innerSensorVao, innerSensorVbo, innerSensorEbo;

    private float[] housingRectanglePositions;
    private float[] chuckPositions;
    private float[] halfCircle1Positions;
    private float[] halfCircle2Positions;
    private float[] outerSensorPositions;
    private float[] innerSensorPositions;

    private uint[] housingRectangleIndices;
    private uint[] chuckIndices;
    private uint[] halfCircle1Indices;
    private uint[] halfCircle2Indices;
    private uint[] outerSensorIndices;
    private uint[] innerSensorIndices;

    private const float PI = 3.14159265358979323846f;

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
        float chuckRadius,

        float halfCircle1StartX,
        float halfCircle1StartY,
        float halfCircle1EndX,
        float halfCircle1EndY,
        float halfCircle2StartX,
        float halfCircle2StartY,
        float halfCircle2EndX,
        float halfCircle2EndY,

        float outerSensorAx,
        float outerSensorAy,
        float outerSensorBx,
        float outerSensorBy,
        float outerSensorCx,
        float outerSensorCy,
        float outerSensorDx,
        float outerSensorDy,

        float innerSensorAx,
        float innerSensorAy,
        float innerSensorBx,
        float innerSensorBy,
        float innerSensorCx,
        float innerSensorCy,
        float innerSensorDx,
        float innerSensorDy
    )
    {
        Scale = scale;
        PosAx = posAx;
        PosAy = posAy;
        PosBx = posBx;
        PosBy = posBy;
        PosCx = posCx;
        PosCy = posCy;
        PosDx = posDx;
        PosDy = posDy;
        ChuckCenterX = chuckCenterX;
        ChuckCenterY = chuckCenterY;
        ChuckRadius = chuckRadius;
        HalfCircle1StartX = halfCircle1StartX;
        HalfCircle1StartY = halfCircle1StartY;
        HalfCircle1EndX = halfCircle1EndX;
        HalfCircle1EndY = halfCircle1EndY;
        HalfCircle2StartX = halfCircle2StartX;
        HalfCircle2StartY = halfCircle2StartY;
        HalfCircle2EndX = halfCircle2EndX;
        HalfCircle2EndY = halfCircle2EndY;
        OuterSensorAx = outerSensorAx;
        OuterSensorAy = outerSensorAy;
        OuterSensorBx = outerSensorBx;
        OuterSensorBy = outerSensorBy;
        OuterSensorCx = outerSensorCx;
        OuterSensorCy = outerSensorCy;
        OuterSensorDx = outerSensorDx;
        OuterSensorDy = outerSensorDy;
        InnerSensorAx = innerSensorAx;
        InnerSensorAy = innerSensorAy;
        InnerSensorBx = innerSensorBx;
        InnerSensorBy = innerSensorBy;
        InnerSensorCx = innerSensorCx;
        InnerSensorCy = innerSensorCy;
        InnerSensorDx = innerSensorDx;
        InnerSensorDy = innerSensorDy;

        numHousingRectangleVertices = 4;
        numChuckVertices = 21; // 20 segments + center
        numHalfCircleVertices = 11; // 10 segments + center
        numOuterSensorVertices = 4;
        numInnerSensorVertices = 4;

        numHousingRectangleIndices = 6;
        numChuckIndices = 3 * 20; // 20 triangles
        numHalfCircleIndices = 3 * 10; // 10 triangles
        numOuterSensorIndices = 6;
        numInnerSensorIndices = 6;

        housingRectanglePositions = new float[]
        {
            posAx * scale, posAy * scale,
            posBx * scale, posBy * scale,
            posCx * scale, posCy * scale,
            posDx * scale, posDy * scale
        };

        housingRectangleIndices = new uint[]
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

        // Initialize half circle 1 positions and indices
        halfCircle1Positions = new float[numHalfCircleVertices * 2];
        halfCircle1Indices = new uint[numHalfCircleIndices];

        // Center of the half circle 1
        float halfCircle1CenterX = (halfCircle1StartX + halfCircle1EndX) / 2;
        float halfCircle1CenterY = (halfCircle1StartY + halfCircle1EndY) / 2;
        float halfCircle1Radius = Formulas.distance(halfCircle1StartX, halfCircle1StartY, halfCircle1EndX, halfCircle1EndY) / 2;

        // Calculate the angle of the line segment
        float angle1 = (float)Math.Atan2(halfCircle1EndY - halfCircle1StartY, halfCircle1EndX - halfCircle1StartX);

        halfCircle1Positions[0] = halfCircle1CenterX * scale;
        halfCircle1Positions[1] = halfCircle1CenterY * scale;

        // Half circle 1 vertices
        for (int i = 0; i <= 10; ++i)
        {
            float theta = angle1 + -PI * i / 10;
            halfCircle1Positions[2 * i] = halfCircle1CenterX * scale + halfCircle1Radius * scale * (float)Math.Cos(theta); // x
            halfCircle1Positions[2 * i + 1] = halfCircle1CenterY * scale + halfCircle1Radius * scale * (float)Math.Sin(theta); // y
        }

        // Half circle 1 indices
        index = 0;
        for (int i = 1; i <= 10; ++i)
        {
            halfCircle1Indices[index++] = 0;
            halfCircle1Indices[index++] = (uint)i;
            halfCircle1Indices[index++] = (uint)(i % 10 + 1);
        }

        // Initialize half circle 2 positions and indices
        halfCircle2Positions = new float[numHalfCircleVertices * 2];
        halfCircle2Indices = new uint[numHalfCircleIndices];

        // Center of the half circle 2
        float halfCircle2CenterX = (halfCircle2StartX + halfCircle2EndX) / 2;
        float halfCircle2CenterY = (halfCircle2StartY + halfCircle2EndY) / 2;
        float halfCircle2Radius = Formulas.distance(halfCircle2StartX, halfCircle2StartY, halfCircle2EndX, halfCircle2EndY) / 2;

        // Calculate the angle of the line segment
        float angle2 = (float)Math.Atan2(halfCircle2EndY - halfCircle2StartY, halfCircle2EndX - halfCircle2StartX);

        halfCircle2Positions[0] = halfCircle2CenterX * scale;
        halfCircle2Positions[1] = halfCircle2CenterY * scale;

        // Half circle 2 vertices
        for (int i = 0; i <= 10; ++i)
        {
            float theta = angle2 + -PI * i / 10;
            halfCircle2Positions[2 * i] = halfCircle2CenterX * scale + halfCircle2Radius * scale * (float)Math.Cos(theta); // x
            halfCircle2Positions[2 * i + 1] = halfCircle2CenterY * scale + halfCircle2Radius * scale * (float)Math.Sin(theta); // y
        }

        // Half circle 2 indices
        index = 0;
        for (int i = 1; i <= 10; ++i)
        {
            halfCircle2Indices[index++] = 0;
            halfCircle2Indices[index++] = (uint)i;
            halfCircle2Indices[index++] = (uint)(i % 10 + 1);
        }

        outerSensorPositions = new float[]
        {
             outerSensorAx * scale,  outerSensorAy* scale,
             outerSensorBx * scale, outerSensorBy * scale,
             outerSensorCx * scale, outerSensorCy * scale,
             outerSensorDx * scale, outerSensorDy * scale
        };

        outerSensorIndices = new uint[]
        {
           0, 1, 2,
           1, 3, 2
        };

        innerSensorPositions = new float[]
        {
             innerSensorAx * scale,  innerSensorAy* scale,
             innerSensorBx * scale, innerSensorBy * scale,
             innerSensorCx * scale, innerSensorCy * scale,
             innerSensorDx * scale, innerSensorDy * scale
        };

        innerSensorIndices = new uint[]
        {
           0, 1, 2,
           1, 3, 2
        };
    }

    ~HA75Aligner()
    {
        GL.DeleteVertexArray(housingRectangleVao);
        GL.DeleteBuffer(housingRectangleVbo);
        GL.DeleteBuffer(housingRectangleEbo);

        GL.DeleteVertexArray(chuckVao);
        GL.DeleteBuffer(chuckVbo);
        GL.DeleteBuffer(chuckEbo);

        GL.DeleteVertexArray(halfCircle1Vao);
        GL.DeleteBuffer(halfCircle1Vbo);
        GL.DeleteBuffer(halfCircle1Ebo);

        GL.DeleteVertexArray(halfCircle2Vao);
        GL.DeleteBuffer(halfCircle2Vbo);
        GL.DeleteBuffer(halfCircle2Ebo);

        GL.DeleteVertexArray(outerSensorVao);
        GL.DeleteBuffer(outerSensorVbo);
        GL.DeleteBuffer(outerSensorEbo);

        GL.DeleteVertexArray(innerSensorVao);
        GL.DeleteBuffer(innerSensorVbo);
        GL.DeleteBuffer(innerSensorEbo);
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

        // Initialize half circle 1 VAO, VBO, and EBO
        GL.GenVertexArrays(1, out halfCircle1Vao);
        GL.GenBuffers(1, out halfCircle1Vbo);
        GL.GenBuffers(1, out halfCircle1Ebo);

        GL.BindVertexArray(halfCircle1Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, halfCircle1Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, halfCircle1Positions.Length * sizeof(float), halfCircle1Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, halfCircle1Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, halfCircle1Indices.Length * sizeof(uint), halfCircle1Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO

        // Initialize half circle 2 VAO, VBO, and EBO
        GL.GenVertexArrays(1, out halfCircle2Vao);
        GL.GenBuffers(1, out halfCircle2Vbo);
        GL.GenBuffers(1, out halfCircle2Ebo);

        GL.BindVertexArray(halfCircle2Vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, halfCircle2Vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, halfCircle2Positions.Length * sizeof(float), halfCircle2Positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, halfCircle2Ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, halfCircle2Indices.Length * sizeof(uint), halfCircle2Indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO

        // Initialize outer sensor VAO, VBO, and EBO
        GL.GenVertexArrays(1, out outerSensorVao);
        GL.GenBuffers(1, out outerSensorVbo);
        GL.GenBuffers(1, out outerSensorEbo);

        GL.BindVertexArray(outerSensorVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, outerSensorVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, outerSensorPositions.Length * sizeof(float), outerSensorPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, outerSensorEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, outerSensorIndices.Length * sizeof(uint), outerSensorIndices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);

        // Initialize inner sensor VAO, VBO, and EBO
        GL.GenVertexArrays(1, out innerSensorVao);
        GL.GenBuffers(1, out innerSensorVbo);
        GL.GenBuffers(1, out innerSensorEbo);

        GL.BindVertexArray(innerSensorVao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, innerSensorVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, innerSensorPositions.Length * sizeof(float), innerSensorPositions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, innerSensorEbo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, innerSensorIndices.Length * sizeof(uint), innerSensorIndices, BufferUsageHint.StaticDraw);

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
            // Draw the perimeter with a black line
            GL.LineWidth(2.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);

            // Draw the perimeter of the housing rectangle
            GL.BindVertexArray(housingRectangleVao);
            GL.DrawElements(PrimitiveType.LineLoop, numHousingRectangleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the perimeter of half circle 1
            GL.BindVertexArray(halfCircle1Vao);
            GL.DrawElements(PrimitiveType.LineLoop, numHalfCircleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the perimeter of half circle 2
            GL.BindVertexArray(halfCircle2Vao);
            GL.DrawElements(PrimitiveType.LineLoop, numHalfCircleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the perimeter of the chuck on top
            GL.BindVertexArray(chuckVao);
            GL.DrawElements(PrimitiveType.LineLoop, numChuckIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the filled areas with a gray color
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            // Draw the filled area of the housing rectangle
            GL.BindVertexArray(housingRectangleVao);
            GL.DrawElements(PrimitiveType.Triangles, numHousingRectangleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the filled area of half circle 1
            GL.BindVertexArray(halfCircle1Vao);
            GL.DrawElements(PrimitiveType.Triangles, numHalfCircleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the filled area of half circle 2
            GL.BindVertexArray(halfCircle2Vao);
            GL.DrawElements(PrimitiveType.Triangles, numHalfCircleIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the perimeter of the chuck on top
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(chuckVao);
            GL.DrawElements(PrimitiveType.LineLoop, numChuckIndices, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);

            // Draw the filled area of the chuck
            GL.BindVertexArray(chuckVao);
            GL.DrawElements(PrimitiveType.Triangles, numChuckIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Draw the perimeter of the outer sensor
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(outerSensorVao);
            GL.DrawElements(PrimitiveType.LineLoop, numOuterSensorIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the filled area of the outer sensor
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(outerSensorVao);
            GL.DrawElements(PrimitiveType.Triangles, numOuterSensorIndices, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            // Draw the perimeter of the inner sensor
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(innerSensorVao);
            GL.DrawElements(PrimitiveType.LineLoop, numInnerSensorIndices, DrawElementsType.UnsignedInt, 0);

            // Draw the filled area of the inner sensor
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(innerSensorVao);
            GL.DrawElements(PrimitiveType.Triangles, numInnerSensorIndices, DrawElementsType.UnsignedInt, 0);

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

