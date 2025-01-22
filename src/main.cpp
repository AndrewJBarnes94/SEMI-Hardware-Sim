#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <cmath>
#include <thread>
#include <atomic>
#include "Shader.h"

std::atomic<float> rotationSpeed(0.0f); // Initial speed set to 0
std::atomic<bool> newInputReceived(false); // Flag to indicate new input
const float M_PI = 3.14159265358979323846f;

void UpdateRotation(float* positions, int numVertices, float angle, float centerX, float centerY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        float x = positions[i] - centerX;
        float y = positions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        positions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        positions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }
}

void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

void HandleUserInput() {
    while (true) {
        float degree;
        std::cout << "Enter rotation degree (positive for clockwise, negative for counterclockwise): ";
        std::cin >> degree;

        float speed = degree * (M_PI / 180.0f); // Convert degree to radians

        rotationSpeed = speed;
        newInputReceived = true; // Set the flag to indicate new input
    }
}

int main() {
    // Initialize GLFW
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return -1;
    }

    // Create window
    GLFWwindow* window = glfwCreateWindow(800, 600, "Rotating Shapes", nullptr, nullptr);
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

    // Base shape data (half-circles on the sides)
    const int numCircleSegments = 20;
    const int numCircleVertices = 2 * (numCircleSegments + 1);
    const int numRectangleVertices = 4;
    float circlePositions[numCircleVertices * 2];
    float rectanglePositions[numRectangleVertices * 2];
    int index = 0;

    // Left half-circle vertices (from -?/2 to ?/2)
    for (int i = 0; i <= numCircleSegments; ++i) {
        float theta = -M_PI / 2 + M_PI * i / numCircleSegments;
        circlePositions[index++] = 0.4f + 0.2f * cos(theta);
        circlePositions[index++] = 0.2f * sin(theta);
    }

    // Right half-circle vertices (from ?/2 to 3?/2)
    for (int i = 0; i <= numCircleSegments; ++i) {
        float theta = M_PI / 2 + M_PI * i / numCircleSegments;
        circlePositions[index++] = -0.4f + 0.2f * cos(theta);
        circlePositions[index++] = 0.2f * sin(theta);
    }

    // Rectangle vertices
    rectanglePositions[0] = 0.4f;  // Top right
    rectanglePositions[1] = 0.2f;
    rectanglePositions[2] = 0.4f;  // Bottom right
    rectanglePositions[3] = -0.2f;
    rectanglePositions[4] = -0.4f; // Bottom left
    rectanglePositions[5] = -0.2f;
    rectanglePositions[6] = -0.4f; // Top left
    rectanglePositions[7] = 0.2f;

    // Calculate the center of the diameter of the right half-circle
    float centerX = -0.4f;
    float centerY = 0.0f;

    // Translate the entire geometry so that the center of the right half-circle is at the origin
    TranslateToCenter(circlePositions, numCircleVertices, -centerX, -centerY);
    TranslateToCenter(rectanglePositions, numRectangleVertices, -centerX, -centerY);

    const int numCircleIndices = 2 * 3 * numCircleSegments;
    const int numRectangleIndices = 6;
    unsigned int circleIndices[numCircleIndices];
    unsigned int rectangleIndices[numRectangleIndices];
    index = 0;

    // Left half-circle indices
    for (int i = 0; i < numCircleSegments; ++i) {
        circleIndices[index++] = 0;
        circleIndices[index++] = i;
        circleIndices[index++] = i + 1;
    }

    // Right half-circle indices
    for (int i = numCircleSegments + 1; i < 2 * numCircleSegments + 1; ++i) {
        circleIndices[index++] = numCircleSegments + 1;
        circleIndices[index++] = i;
        circleIndices[index++] = i + 1;
    }

    // Rectangle indices
    rectangleIndices[0] = 0; // Top right
    rectangleIndices[1] = 1; // Bottom right
    rectangleIndices[2] = 2; // Bottom left
    rectangleIndices[3] = 0; // Top right
    rectangleIndices[4] = 2; // Bottom left
    rectangleIndices[5] = 3; // Top left

    // Vertex Array Object and Buffer for circles
    unsigned int circleVao, circleVbo, circleEbo;
    glGenVertexArrays(1, &circleVao);
    glGenBuffers(1, &circleVbo);
    glGenBuffers(1, &circleEbo);

    // Initialize VAO and VBO for circles
    glBindVertexArray(circleVao);
    glBindBuffer(GL_ARRAY_BUFFER, circleVbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(circlePositions), circlePositions, GL_DYNAMIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, circleEbo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(circleIndices), circleIndices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr);
    glEnableVertexAttribArray(0);

    // Vertex Array Object and Buffer for rectangle
    unsigned int rectangleVao, rectangleVbo, rectangleEbo;
    glGenVertexArrays(1, &rectangleVao);
    glGenBuffers(1, &rectangleVbo);
    glGenBuffers(1, &rectangleEbo);

    // Initialize VAO and VBO for rectangle
    glBindVertexArray(rectangleVao);
    glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo);
    glBufferData(GL_ARRAY_BUFFER, sizeof(rectanglePositions), rectanglePositions, GL_DYNAMIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rectangleEbo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(rectangleIndices), rectangleIndices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr);
    glEnableVertexAttribArray(0);

    // Shader
    Shader shader("res/shaders/basic.shader");
    shader.Bind();

    int location = shader.GetUniformLocation("u_Color");

    // Start user input thread
    std::thread inputThread(HandleUserInput);

    // Main render loop
    while (!glfwWindowShouldClose(window)) {
        glClear(GL_COLOR_BUFFER_BIT);

        if (newInputReceived) {
            // Update positions with rotation based on new input
            UpdateRotation(circlePositions, numCircleVertices, rotationSpeed, 0.0f, 0.0f);
            UpdateRotation(rectanglePositions, numRectangleVertices, rotationSpeed, 0.0f, 0.0f);

            // Update VBOs
            glBindBuffer(GL_ARRAY_BUFFER, circleVbo);
            glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(circlePositions), circlePositions);

            glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo);
            glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(rectanglePositions), rectanglePositions);

            newInputReceived = false; // Reset the flag
        }

        // Set color to metallic gray/silver
        glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f);

        // Draw the circles
        glBindVertexArray(circleVao);
        glDrawElements(GL_TRIANGLES, numCircleIndices, GL_UNSIGNED_INT, nullptr);

        // Draw the rectangle
        glBindVertexArray(rectangleVao);
        glDrawElements(GL_TRIANGLES, numRectangleIndices, GL_UNSIGNED_INT, nullptr);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }

    inputThread.join();
    glfwTerminate();
    return 0;
}

