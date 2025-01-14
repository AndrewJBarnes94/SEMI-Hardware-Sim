#pragma once
#ifndef RENDERER_H
#define RENDERER_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>

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
};

#endif // RENDERER_H
