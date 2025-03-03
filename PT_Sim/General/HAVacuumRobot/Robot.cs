using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using PT_Sim;

public class Robot
{
    private Shader shader;
    private float scale;
    private float posX, posY, initialRotationRadians;
    private float angle1, angle2, angle3;
    private bool newInputReceived;
    private RobotArmEndEffector endEffector;
    private RobotArmAppendage appendage_1, appendage_2;
    private Base vacuumSeal;

    private const float PI = 3.14159265358979323846f;

    public Robot(ref float angle1, ref float angle2, ref float angle3, ref bool newInputReceived, float scale)
    {
        this.shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");
        this.angle1 = angle1;
        this.angle2 = angle2;
        this.angle3 = angle3;
        this.newInputReceived = newInputReceived;
        this.scale = scale;
        this.vacuumSeal = new Base(scale);
        this.appendage_1 = new RobotArmAppendage(scale);
        this.appendage_2 = new RobotArmAppendage(scale);
        this.endEffector = new RobotArmEndEffector(scale);
    }

    public void Initialize(float posX, float posY, float initialRotationDegrees)
    {
        this.posX = posX;
        this.posY = posY;
        this.initialRotationRadians = initialRotationDegrees * (PI / 180.0f);

        appendage_1.Initialize();
        appendage_2.Initialize();
        endEffector.Initialize();
        vacuumSeal.Initialize();

        shader.Bind();
    }

    public void Update(float angle1, float angle2, float angle3)
    {
        // Convert angles to radians
        float angle1Rad = angle1 * (PI / 180.0f);
        float angle2Rad = angle2 * (PI / 180.0f);
        float angle3Rad = angle3 * (PI / 180.0f);

        appendage_1.UpdateRotation(angle1Rad, 0.0f, 0.0f);
        var redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
        appendage_2.UpdateRotation(angle2Rad, redDotPosition_1.Item1, redDotPosition_1.Item2);
        appendage_2.TranslateToPosition(redDotPosition_1.Item1, redDotPosition_1.Item2);

        var redDotPosition_2 = appendage_2.CalculateRedDotPosition("left");
        endEffector.UpdateRotation(angle3Rad, redDotPosition_2.Item1, redDotPosition_2.Item2);
        endEffector.TranslateToPosition(redDotPosition_2.Item1, redDotPosition_2.Item2);

        // Reset newInputReceived to false after processing the update
        newInputReceived = false;
    }

    public void Render()
    {
        shader.Bind();

        vacuumSeal.Render(shader);
        appendage_1.Render(shader);
        appendage_2.Render(shader);
        endEffector.Render(shader);

        shader.Unbind();

        var redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
        var redDotPosition_2 = appendage_2.CalculateRedDotPosition("left");

        //RenderDot(redDotPosition_1.Item1, redDotPosition_1.Item2, 0.01f, 1.0f, 0.0f, 0.0f);
        //RenderDot(redDotPosition_2.Item1, redDotPosition_2.Item2, 0.01f, 1.0f, 0.0f, 0.0f);
    }

    private void RenderDot(float x, float y, float size, float r, float g, float b)
    {
        int location = shader.GetUniformLocation("u_Color");
        GL.Uniform4(location, r, g, b, 1.0f);

        float[] dotVertices = {
            x - size, y - size,
            x + size, y - size,
            x + size, y + size,
            x - size, y + size
        };

        uint[] dotIndices = { 0, 1, 2, 2, 3, 0 };

        int vao, vbo, ebo;
        GL.GenVertexArrays(1, out vao);
        GL.GenBuffers(1, out vbo);
        GL.GenBuffers(1, out ebo);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, dotVertices.Length * sizeof(float), dotVertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, dotIndices.Length * sizeof(uint), dotIndices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), IntPtr.Zero);
        GL.EnableVertexAttribArray(0);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo);
    }

    public void SetAngles(float angle1, float angle2, float angle3)
    {
        this.angle1 = angle1;
        this.angle2 = angle2;
        this.angle3 = angle3;
        newInputReceived = true;
        Console.WriteLine($"SetAngles called: angle1 = {angle1}, angle2 = {angle2}, angle3 = {angle3}");
    }
}
