using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;
using PT_Sim.General.HAVacuumRobot;

public class HA600 : GLControl
{
    private Shader _shader;

    private HA600TMChamber _chamber;

    private SlitValvePositions _slitValvePositions;

    private SlitValve _slitValve1;
    private SlitValve _slitValve2;
    private SlitValve _slitValve3;
    private SlitValve _slitValve4;
    private SlitValve _slitValve5;
    private SlitValve _slitValve6;

    private ProcessModulePositions _processModulePositions;

    private ProcessModule _processModule1;
    private ProcessModule _processModule2;
    private ProcessModule _processModule3;
    private ProcessModule _processModule4;

    private CLP _AL;
    private CLP _BL;


    public HA600()
        : base(new GraphicsMode(32, 24, 0, 4))
    {
        this.Dock = DockStyle.Fill;
        this.Load += OnLoad;
        this.Paint += OnPaint;
        this.Resize += OnResize;
    }

    private CLPPositions _CLPPositions;

    private void OnLoad(object sender, EventArgs e)
    {
        // Set Background Color (Dark Tech Grey-Blue)
        GL.ClearColor(0.8f, 0.8f, 1.0f, 1.0f);

        // Load and compile shaders
        _shader = new Shader("vertexShader.glsl", "fragmentShader.glsl");

        float scale = 0.8f;

        _chamber = new HA600TMChamber(scale);
        _chamber.Initialize();

        _slitValvePositions = new SlitValvePositions(_chamber);

        _slitValve1 = _slitValvePositions.GetSlitValve1();
        _slitValve1.Initialize();

        _slitValve2 = _slitValvePositions.GetSlitValve2();
        _slitValve2.Initialize();

        _slitValve3 = _slitValvePositions.GetSlitValve3();
        _slitValve3.Initialize();

        _slitValve4 = _slitValvePositions.GetSlitValve4();
        _slitValve4.Initialize();

        _slitValve5 = _slitValvePositions.GetSlitValve5();
        _slitValve5.Initialize();

        _slitValve6 = _slitValvePositions.GetSlitValve6();
        _slitValve6.Initialize();

        _processModulePositions = new ProcessModulePositions(
            _slitValve1,
            _slitValve2,
            _slitValve3,
            _slitValve4
        );

        _processModule1 = _processModulePositions.GetProcessModule1();
        _processModule1.Initialize();

        _processModule2 = _processModulePositions.GetProcessModule2();
        _processModule2.Initialize();

        _processModule3 = _processModulePositions.GetProcessModule3();
        _processModule3.Initialize();

        _processModule4 = _processModulePositions.GetProcessModule4();
        _processModule4.Initialize();

        _CLPPositions = new CLPPositions(_slitValve5, _slitValve6);


        _AL = _CLPPositions.GetAL();
        _AL.Initialize();

        _BL = _CLPPositions.GetBL();
        _BL.Initialize();
    }


    private void OnPaint(object sender, PaintEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _processModule1.Render(_shader);
        _processModule2.Render(_shader);
        _processModule3.Render(_shader);
        _processModule4.Render(_shader);
        _AL.Render(_shader);
        _BL.Render(_shader);

        _slitValve1.Render(_shader);
        _slitValve2.Render(_shader);
        _slitValve4.Render(_shader);
        _slitValve3.Render(_shader);
        _slitValve5.Render(_shader);
        _slitValve6.Render(_shader);
        
        _chamber.Render(_shader);

        SwapBuffers();
    }

    private void OnResize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
    }
}
