using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

public class Shader
{
    private int handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        // Load vertex and fragment shaders
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        // Compile vertex shader
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompileStatus(vertexShader);

        // Compile fragment shader
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompileStatus(fragmentShader);

        // Link shaders into a program
        handle = GL.CreateProgram();
        GL.AttachShader(handle, vertexShader);
        GL.AttachShader(handle, fragmentShader);
        GL.LinkProgram(handle);
        CheckProgramLinkStatus(handle);

        // Clean up shaders as they are no longer needed after linking
        GL.DetachShader(handle, vertexShader);
        GL.DetachShader(handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Bind()
    {
        GL.UseProgram(handle);
    }

    public void Unbind()
    {
        GL.UseProgram(0);
    }

    public int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(handle, name);
    }

    private void CheckShaderCompileStatus(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == (int)All.False)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation failed: {infoLog}");
        }
    }

    private void CheckProgramLinkStatus(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
        if (status == (int)All.False)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Program linking failed: {infoLog}");
        }
    }

    ~Shader()
    {
        GL.DeleteProgram(handle);
    }
}
