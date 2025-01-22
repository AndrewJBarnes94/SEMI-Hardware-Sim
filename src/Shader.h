#pragma once

#include <string>

class Shader {
public:
    Shader(const std::string& filepath);  // Constructor: Loads and compiles shaders from a file
    ~Shader();                            // Destructor: Cleans up the shader program

    void Bind() const;                    // Binds the shader program for use
    void Unbind() const;                  // Unbinds the shader program
    int GetUniformLocation(const std::string& name) const; // Get location of a uniform variable

private:
    struct ShaderProgramSource {
        std::string VertexSource;
        std::string FragmentSource;
    };

    unsigned int shader;                  // Shader program ID
    ShaderProgramSource ParseShader(const std::string& filepath) const;  // Parse shader source
    unsigned int CompileShader(unsigned int type, const std::string& source) const; // Compile a shader
    unsigned int CreateShader(const std::string& vertexShader, const std::string& fragmentShader) const; // Create shader program
};

