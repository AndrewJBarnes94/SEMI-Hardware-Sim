using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

public class GLRenderPanel : GLControl
{
    private int _vertexBufferObject, _vertexArrayObject, _elementBufferObject;
    private int _shaderProgram;

    // Vertex Data (Positions)
    private readonly float[] _vertices =
    {
        // Positions        // Colors (RGB)
        0.0f,  0.5f, 0.0f,   1.0f, 0.0f, 0.0f,  // Top (Red)
       -0.5f, -0.5f, 0.0f,   0.0f, 1.0f, 0.0f,  // Bottom-left (Green)
        0.5f, -0.5f, 0.0f,   0.0f, 0.0f, 1.0f   // Bottom-right (Blue)
    };

    // Index Data (EBO)
    private readonly uint[] _indices =
    {
        0, 1, 2 // Triangle
    };

    public GLRenderPanel()
        : base(new GraphicsMode(32, 24, 0, 4))
    {
        this.Dock = DockStyle.Fill;
        this.Load += OnLoad;
        this.Paint += OnPaint;
        this.Resize += OnResize;
    }

    private void OnLoad(object sender, EventArgs e)
    {
        // Set Background Color (Dark Tech Grey-Blue)
        GL.ClearColor(0.1f, 0.25f, 0.79f, 1.0f);

        // Generate VAO
        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);

        // Generate VBO (Vertex Buffer)
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        // Generate EBO (Element Buffer)
        _elementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        // Compile Shaders
        _shaderProgram = CreateShaderProgram();
        GL.UseProgram(_shaderProgram);

        // Set Vertex Attributes (Position + Color)
        int positionLocation = GL.GetAttribLocation(_shaderProgram, "aPosition");
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(positionLocation);

        int colorLocation = GL.GetAttribLocation(_shaderProgram, "aColor");
        GL.VertexAttribPointer(colorLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(colorLocation);

        // Unbind Buffers
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }

    private void OnResize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
    }

    private int CreateShaderProgram()
    {
        // Vertex Shader
        string vertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec3 aColor;
        out vec3 vertexColor;
        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            vertexColor = aColor; // Pass color to fragment shader
        }";

        int vertexShader = CompileShader(vertexShaderSource, ShaderType.VertexShader);

        // Fragment Shader
        string fragmentShaderSource = @"
        #version 330 core
        in vec3 vertexColor;
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(vertexColor, 1.0); // Use vertex color
        }";

        int fragmentShader = CompileShader(fragmentShaderSource, ShaderType.FragmentShader);

        // Link Shader Program
        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(shaderProgram);
            MessageBox.Show($"Shader Program Linking Error:\n{infoLog}", "Shader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Cleanup
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }

    private int CompileShader(string source, ShaderType type)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            MessageBox.Show($"Shader Compilation Error ({type}):\n{infoLog}", "Shader Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return shader;
    }
}
