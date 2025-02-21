using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

public class Chamber_600
{
    private float scale;
    private int numVertices;
    private int numIndices;
    private int numExtensionVertices1;
    private int numExtensionIndices1;
    private int numExtensionVertices2;
    private int numExtensionIndices2;

    private float[] positions;
    private uint[] indices;
    private float[] extensionPositions1;
    private uint[] extensionIndices1;
    private float[] extensionPositions2;
    private uint[] extensionIndices2;

    private int vao, vbo, ebo;
    private int extensionVao1, extensionVbo1, extensionEbo1;
    private int extensionVao2, extensionVbo2, extensionEbo2;

    public Chamber_600(float scale)
    {
        this.scale = scale;
        numVertices = 7; // 6 vertices for the hexagon + 1 for the center
        numIndices = 18; // 6 triangles (3 indices each)

        numExtensionVertices1 = 4;
        numExtensionIndices1 = 6;

        numExtensionVertices2 = 4;
        numExtensionIndices2 = 6;

        positions = new float[numVertices * 2]; // Each vertex has x and y coordinates
        indices = new uint[numIndices];

        extensionIndices1 = new uint[numExtensionIndices1];
        extensionPositions1 = new float[numExtensionVertices1 * 2];

        extensionIndices2 = new uint[numExtensionIndices2];
        extensionPositions2 = new float[numExtensionVertices2 * 2];
    }

    ~Chamber_600()
    {
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);

        GL.DeleteVertexArray(extensionVao1);
        GL.DeleteBuffer(extensionVbo1);
        GL.DeleteBuffer(extensionEbo1);

        GL.DeleteVertexArray(extensionVao2);
        GL.DeleteBuffer(extensionVbo2);
        GL.DeleteBuffer(extensionEbo2);
    }

    public void Initialize()
    {
        // Define vertices for a hexagon with rotation
        const float angleIncrement = 2.0f * (float)Math.PI / 6.0f; // 360° / 6 sides
        const float offset = (float)Math.PI / 6.0f; // Rotate by 30° so sides are vertical
        positions[0] = 0.0f; // Center x
        positions[1] = 0.0f; // Center y

        for (int i = 0; i < 6; ++i)
        {
            float angle = i * angleIncrement + offset;
            positions[(i + 1) * 2] = (float)Math.Cos(angle) * scale;
            positions[(i + 1) * 2 + 1] = (float)Math.Sin(angle) * scale;
        }

        // Define indices for 6 triangles forming the hexagon
        int index = 0;
        for (int i = 1; i <= 6; ++i)
        {
            indices[index++] = 0; // Center vertex
            indices[index++] = (uint)i;
            indices[index++] = (uint)((i % 6) + 1);
        }

        extensionPositions1[0] = -0.411362f * scale / 0.475f;
        extensionPositions1[1] = -0.1775f * scale / 0.475f;
        extensionPositions1[2] = 0.0f * scale / 0.475f;
        extensionPositions1[3] = -0.415f * scale / 0.475f;
        extensionPositions1[4] = 0.0f * scale / 0.475f;
        extensionPositions1[5] = -0.56f * scale / 0.475f;
        extensionPositions1[6] = -0.411362f * scale / 0.475f;
        extensionPositions1[7] = -0.38f * scale / 0.475f;

        extensionIndices1[0] = 0;
        extensionIndices1[1] = 1;
        extensionIndices1[2] = 2;
        extensionIndices1[3] = 0;
        extensionIndices1[4] = 2;
        extensionIndices1[5] = 3;

        extensionPositions2[0] = 0.411362f * scale / 0.475f;
        extensionPositions2[1] = -0.1775f * scale / 0.475f;
        extensionPositions2[2] = 0.0f * scale / 0.475f;
        extensionPositions2[3] = -0.415f * scale / 0.475f;
        extensionPositions2[4] = 0.0f * scale / 0.475f;
        extensionPositions2[5] = -0.56f * scale / 0.475f;
        extensionPositions2[6] = 0.411362f * scale / 0.475f;
        extensionPositions2[7] = -0.38f * scale / 0.475f;

        extensionIndices2[0] = 0;
        extensionIndices2[1] = 1;
        extensionIndices2[2] = 2;
        extensionIndices2[3] = 0;
        extensionIndices2[4] = 2;
        extensionIndices2[5] = 3;

        // Ensure buffers are initialized correctly
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        if (vao == 0 || vbo == 0 || ebo == 0)
        {
            Console.WriteLine("Error: VAO, VBO, or EBO not initialized correctly");
            return;
        }

        // Generate and bind VAO for the hexagon
        GL.BindVertexArray(vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, numVertices * 2 * sizeof(float), positions, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, numIndices * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0); // Unbind VAO

        // Initialize extension1 VAO
        GL.GenVertexArrays(1, out extensionVao1);
        GL.GenBuffers(1, out extensionVbo1);
        GL.GenBuffers(1, out extensionEbo1);

        GL.BindVertexArray(extensionVao1);
        GL.BindBuffer(BufferTarget.ArrayBuffer, extensionVbo1);
        GL.BufferData(BufferTarget.ArrayBuffer, numExtensionVertices1 * 2 * sizeof(float), extensionPositions1, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, extensionEbo1);
        GL.BufferData(BufferTarget.ElementArrayBuffer, numExtensionIndices1 * sizeof(uint), extensionIndices1, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindVertexArray(0);

        // Initialize extension2 VAO
        GL.GenVertexArrays(1, out extensionVao2);
        GL.GenBuffers(1, out extensionVbo2);
        GL.GenBuffers(1, out extensionEbo2);

        GL.BindVertexArray(extensionVao2);
        GL.BindBuffer(BufferTarget.ArrayBuffer, extensionVbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, numExtensionVertices2 * 2 * sizeof(float), extensionPositions2, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, extensionEbo2);
        GL.BufferData(BufferTarget.ElementArrayBuffer, numExtensionIndices2 * sizeof(uint), extensionIndices2, BufferUsageHint.StaticDraw);
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
            GL.LineWidth(5.0f);
            GL.Uniform4(location, 0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.LineLoop, 6, DrawElementsType.UnsignedInt, 0);

            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, 0);

            // Draw extension1
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(extensionVao1);
            GL.DrawElements(PrimitiveType.Triangles, numExtensionIndices1, DrawElementsType.UnsignedInt, 0);

            // Draw extension2
            GL.Uniform4(location, 0.75f, 0.75f, 0.75f, 1.0f);
            GL.BindVertexArray(extensionVao2);
            GL.DrawElements(PrimitiveType.Triangles, numExtensionIndices2, DrawElementsType.UnsignedInt, 0);

            List<float> positionMap = GetPositionMap("bottom");
            if (positionMap.Count > 0)
            {
                List<float> position = positionMap;

                int pointVao, pointVbo;
                GL.GenVertexArrays(1, out pointVao);
                GL.GenBuffers(1, out pointVbo);

                GL.BindVertexArray(pointVao);
                GL.BindBuffer(BufferTarget.ArrayBuffer, pointVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, position.Count * sizeof(float), position.ToArray(), BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                GL.PointSize(15.0f); // Set the point size to make it larger
                GL.Uniform4(location, 1.0f, 0.0f, 0.0f, 1.0f); // Set color to red
                GL.DrawArrays(PrimitiveType.Points, 0, 1);

                GL.BindVertexArray(0);
                GL.DeleteVertexArray(pointVao);
                GL.DeleteBuffer(pointVbo);
            }

            ErrorCode err;
            while ((err = GL.GetError()) != ErrorCode.NoError)
            {
                Console.WriteLine("OpenGL Error: " + err);
            }

            GL.BindVertexArray(0);
            shader.Unbind();
        }
    }

    public List<float> GetPositions()
    {
        List<float> allPositions = new List<float>();

        allPositions.AddRange(positions);
        allPositions.AddRange(extensionPositions1);
        allPositions.AddRange(extensionPositions2);

        return allPositions;
    }

    public List<float> GetPositionMap(string point)
    {
        List<float> positions = GetPositions();

        Dictionary<string, List<float>> positionMap = new Dictionary<string, List<float>>();
        if (point == "center")
        {
            positionMap["center"] = new List<float> { positions[0], positions[1] };
            return positionMap["center"];
        }
        else if (point == "topRight")
        {
            positionMap["topRight"] = new List<float> { positions[2], positions[3] };
            return positionMap["topRight"];
        }
        else if (point == "top")
        {
            positionMap["top"] = new List<float> { positions[4], positions[5] };
            return positionMap["top"];
        }
        else if (point == "topLeft")
        {
            positionMap["topLeft"] = new List<float> { positions[6], positions[7] };
            return positionMap["topLeft"];
        }
        else if (point == "bottomLeft")
        {
            positionMap["bottomLeft"] = new List<float> { positions[20], positions[21] };
            return positionMap["bottomLeft"];
        }
        else if (point == "bottom")
        {
            positionMap["bottom"] = new List<float> { positions[26], positions[27] };
            return positionMap["bottom"];
        }
        else if (point == "bottomRight")
        {
            positionMap["bottomRight"] = new List<float> { positions[28], positions[29] };
            return positionMap["bottomRight"];
        }
        else
        {
            return new List<float>();
        }
    }
}
