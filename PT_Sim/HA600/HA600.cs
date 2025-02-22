using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PT_Sim;

public class HA600 : GLControl
{
    private Shader _shader;
    private HA600TMChamber _chamber;

    public HA600()
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
        GL.ClearColor(0.8f, 0.8f, 1.0f, 1.0f);

        // Load and compile shaders
        _shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");

        float scale = 1.0f;

        // Initialize Chamber_600
        _chamber = new HA600TMChamber(scale);
        _chamber.Initialize();

        Logger.Log(_chamber.GetPositionMap("center"));
        Logger.Log(_chamber.GetPositionMap("top"));
        Logger.Log(_chamber.GetPositionMap("topRight"));
        Logger.Log(_chamber.GetPositionMap("bottomRight"));
        Logger.Log(_chamber.GetPositionMap("bottom"));
        Logger.Log(_chamber.GetPositionMap("bottomLeft"));
        Logger.Log(_chamber.GetPositionMap("topLeft"));

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
