#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <cmath>
#include "Shader.h"

const int RECT_COUNT = 3; // Number of rectangles

void UpdateRectangleRotation(float* positions, float angle, float centerX, float centerY) {
    for (int i = 0; i < 8; i += 2) {
        float x = positions[i] - centerX;
        float y = positions[i + 1] - centerY;

        // Apply rotation
        positions[i] = cos(angle) * x - sin(angle) * y + centerX; // New x
        positions[i + 1] = sin(angle) * x + cos(angle) * y + centerY; // New y
    }
}

int main() {
    // Initialize GLFW
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return -1;
    }

    // Create window
    GLFWwindow* window = glfwCreateWindow(800, 600, "Rotating Rectangles", nullptr, nullptr);
    if (!window) {
        std::cerr << "Failed to create GLFW window." << std::endl;
        glfwTerminate();
        return -1;
    }

    glfwMakeContextCurrent(window);
    glfwSwapInterval(1); // Enable VSync

    // Initialize GLEW
    if (glewInit() != GLEW_OK) {
        std::cerr << "Failed to initialize GLEW." << std::endl;
        return -1;
    }

    std::cout << "OpenGL Version: " << glGetString(GL_VERSION) << std::endl;

    // Base rectangle data (used as a template for all rectangles)
    float basePositions[] = {
        -0.2f, -0.2f,
         0.2f, -0.2f,
         0.2f,  0.2f,
        -0.2f,  0.2f
    };

    unsigned int indices[] = { 0, 1, 2, 2, 3, 0 };

    // Vertex Array Objects and Buffers for each rectangle
    unsigned int vaos[RECT_COUNT], vbos[RECT_COUNT];
    glGenVertexArrays(RECT_COUNT, vaos);
    glGenBuffers(RECT_COUNT, vbos);

    // Separate position arrays and rotation angles for each rectangle
    float positions[RECT_COUNT][8];
    float angles[RECT_COUNT] = { 0.0f, 0.0f, 0.0f }; // Initial rotation angles
    float rotationSpeeds[RECT_COUNT] = { 0.01f, 0.015f, 0.02f }; // Different speeds

    // Rectangle centers
    float centers[RECT_COUNT][2] = {
        {-0.5f, 0.0f}, // Left
        { 0.0f, 0.0f}, // Center
        { 0.5f, 0.0f}  // Right
    };

    // Initialize VAOs and VBOs
    for (int i = 0; i < RECT_COUNT; i++) {
        // Copy base rectangle positions for this rectangle
        std::copy(std::begin(basePositions), std::end(basePositions), positions[i]);

        // Bind VAO and VBO
        glBindVertexArray(vaos[i]);
        glBindBuffer(GL_ARRAY_BUFFER, vbos[i]);
        glBufferData(GL_ARRAY_BUFFER, sizeof(positions[i]), positions[i], GL_DYNAMIC_DRAW);

        // Element Buffer (shared for all)
        unsigned int ebo;
        glGenBuffers(1, &ebo);
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices), indices, GL_STATIC_DRAW);

        glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr);
        glEnableVertexAttribArray(0);
    }

    // Shader
    Shader shader("res/shaders/basic.shader");
    shader.Bind();

    int location = shader.GetUniformLocation("u_Color");

    // Main render loop
    while (!glfwWindowShouldClose(window)) {
        glClear(GL_COLOR_BUFFER_BIT);

        for (int i = 0; i < RECT_COUNT; i++) {
            // Update positions with rotation
            UpdateRectangleRotation(positions[i], angles[i], centers[i][0], centers[i][1]);
            angles[i] += rotationSpeeds[i];

            // Reset angle to stay within 0 to 2?
            int M_PI = 3.14159265358979323846;
            if (angles[i] > 2.0f * M_PI) angles[i] -= 2.0f * M_PI;

            // Update VBO
            glBindBuffer(GL_ARRAY_BUFFER, vbos[i]);
            glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(positions[i]), positions[i]);

            // Set color based on rectangle index
            float r = (i == 0) ? 1.0f : 0.5f;
            float g = (i == 1) ? 1.0f : 0.5f;
            float b = (i == 2) ? 1.0f : 0.5f;
            glUniform4f(location, r, g, b, 1.0f);

            // Draw the rectangle
            glBindVertexArray(vaos[i]);
            glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, nullptr);
        }

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    glfwTerminate();
    return 0;
}
