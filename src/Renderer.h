#pragma once
#ifndef RENDERER_H
#define RENDERER_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <fstream>
#include <string>
#include <sstream>

class Renderer {
private:
    GLFWwindow* window;
    unsigned int buffer;
    const int windowWidth;
    const int windowHeight;
    const char* windowTitle;

public:
    Renderer(int width = 640, int height = 480, const char* title = "Hello World");
    bool initialize();
    void setup();
    void renderLoop();
    void cleanup();
    ~Renderer();
    bool GLLogCall(const char* function, const char* file, int line);
	void GLClearError();
    struct ShaderProgramSource {
        std::string VertexSource;
        std::string FragmentSource;
    };
	ShaderProgramSource ParseShader(const std::string& filepath);
    unsigned int CompileShader(unsigned int type, const std::string& source);
    unsigned int CreateShader(const std::string& vertexShader, const std::string& fragmentShader);
};

#endif // RENDERER_H
