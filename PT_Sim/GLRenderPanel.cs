using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

public class GLRenderPanel : GLControl
{
    private Shader _shader;
    private Chamber_600 _chamber;

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

        // Load and compile shaders
        _shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");

        // Initialize Chamber_600
        _chamber = new Chamber_600(1.0f);
        _chamber.Initialize();
    }

    private void OnPaint(object sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Render Chamber_600
        _chamber.Render(_shader);

        SwapBuffers();
    }

    private void OnResize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
    }
}
