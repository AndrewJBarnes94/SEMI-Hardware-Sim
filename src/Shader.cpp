#include "Shader.h"
#include <GL/glew.h>
#include <iostream>
#include <fstream>
#include <sstream>

Shader::Shader(const std::string& filepath) {
    ShaderProgramSource source = ParseShader(filepath);
    shader = CreateShader(source.VertexSource, source.FragmentSource);
}

Shader::~Shader() {
	// Frees the memory and invalidates the name associated with the program object
    glDeleteProgram(shader);
}

void Shader::Bind() const {
	// Installs the program object specified by program as part of current rendering state
    glUseProgram(shader);
}

void Shader::Unbind() const {
    glUseProgram(0);
}

int Shader::GetUniformLocation(const std::string& name) const {
	// Returns the location of a uniform variable
    int location = glGetUniformLocation(shader, name.c_str());
    if (location == -1) {
        std::cerr << "Warning: Uniform '" << name << "' not found or not active in shader program." << std::endl;
    }
    return location;
}

Shader::ShaderProgramSource Shader::ParseShader(const std::string& filepath) const {
    std::ifstream stream(filepath);
    if (!stream) {
        throw std::runtime_error("Failed to open shader file: " + filepath);
    }

    std::stringstream ss[2];
    std::string line;
    enum class ShaderType { NONE = -1, VERTEX = 0, FRAGMENT = 1 };
    ShaderType type = ShaderType::NONE;

    while (getline(stream, line)) {
        if (line.find("#shader") != std::string::npos) {
            if (line.find("vertex") != std::string::npos) {
                type = ShaderType::VERTEX;
            }
            else if (line.find("fragment") != std::string::npos) {
                type = ShaderType::FRAGMENT;
            }
        }
        else if (type != ShaderType::NONE) {
            ss[static_cast<int>(type)] << line << '\n';
        }
    }

    return { ss[0].str(), ss[1].str() };
}

unsigned int Shader::CompileShader(unsigned int type, const std::string& source) const {
	// Creates an empty shader object and returns a non-zero value by which it can be referenced
    unsigned int id = glCreateShader(type);
    const char* src = source.c_str();
	// Replaces the source code in a shader object
    glShaderSource(id, 1, &src, nullptr);
	// Compiles the source code strings that have been stored in the shader object
    glCompileShader(id);

    int result;
	// Returns a parameter from a shader object
    glGetShaderiv(id, GL_COMPILE_STATUS, &result);
	// If the compilation failed
    if (result == GL_FALSE) {
        int length;
        glGetShaderiv(id, GL_INFO_LOG_LENGTH, &length);
        char* message = new char[length];
        glGetShaderInfoLog(id, length, &length, message);
        std::cerr << "Failed to compile " << (type == GL_VERTEX_SHADER ? "vertex" : "fragment") << " shader!" << std::endl;
        std::cerr << message << std::endl;
        delete[] message;
        glDeleteShader(id);
        return 0;
    }

    return id;
}

unsigned int Shader::CreateShader(const std::string& vertexShader, const std::string& fragmentShader) const {
    unsigned int program = glCreateProgram();
    unsigned int vs = CompileShader(GL_VERTEX_SHADER, vertexShader);
    unsigned int fs = CompileShader(GL_FRAGMENT_SHADER, fragmentShader);

    glAttachShader(program, vs);
    glAttachShader(program, fs);
    glLinkProgram(program);
    glValidateProgram(program);

    glDeleteShader(vs);
    glDeleteShader(fs);

    return program;
}
