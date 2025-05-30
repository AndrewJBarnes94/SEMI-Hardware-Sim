﻿using System;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PT_Sim;
using PT_Sim.General;

public class HA600 : GLControl
{
    private Shader _shader;

    private HA600TMChamber _chamber;

    private HA75AlignerPositions _alignerBLPositions;
    private HA75Aligner _alignerBL;
    private HA75Aligner _alignerAL;

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

    private CLPCassettePositions _CLPCassettePositions;

    private CLPCassette _cassette1;
    private CLPCassette _cassette2;

    private Robot _robot;
    private float angle1 = 45.0f;
    private float angle2 = 90.0f;
    private float angle3 = 180.0f;
    private bool newInputReceived = false;

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

        _alignerBLPositions = new HA75AlignerPositions(_chamber);

        _alignerBL = _alignerBLPositions.GetHA75Aligner();
        _alignerBL.Initialize();

        _alignerAL = _alignerBLPositions.GetMirroredHA75Aligner(_alignerBL);
        _alignerAL.Initialize();

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

        _CLPCassettePositions = new CLPCassettePositions(_AL, _BL);
        
        _cassette1 = _CLPCassettePositions.GetALCassettePlatform();
        _cassette1.Initialize();

        _cassette2 = _CLPCassettePositions.GetBLCassettePlatform();
        _cassette2.Initialize();

        // Initialize Robot
        _robot = new Robot(ref angle1, ref angle2, ref angle3, ref newInputReceived, scale * 0.9f);
        _robot.Initialize(0.0f, 0.0f, 0.0f);
        SetRobotAngles(30.0f, -30.0f, -90.0f);
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

        _cassette1.Render(_shader);
        _cassette2.Render(_shader);

        _slitValve1.Render(_shader);
        _slitValve2.Render(_shader);
        _slitValve4.Render(_shader);
        _slitValve3.Render(_shader);
        _slitValve5.Render(_shader);
        _slitValve6.Render(_shader);

        _chamber.Render(_shader);

        _alignerAL.Render(_shader);
        _alignerBL.Render(_shader);

        _cassette1.Render(_shader);
        _cassette2.Render(_shader);

        // Update and render the robot
        if (newInputReceived)
        {
            _robot.Update(angle1, angle2, angle3);
            newInputReceived = false;
        }
        _robot.Render();

        SwapBuffers();
    }

    private void OnResize(object sender, EventArgs e)
    {
        GL.Viewport(0, 0, this.ClientSize.Width, this.ClientSize.Height);
    }

    public void SetRobotAngles(float angle1, float angle2, float angle3)
    {
        this.angle1 = angle1;
        this.angle2 = angle2;
        this.angle3 = angle3;
        newInputReceived = true;
        Invalidate(); // Force repaint to update the robot
    }
}
