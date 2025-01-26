// File: src/RobotArm.cpp
#include "RobotArm.h"
#include <iostream>
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArm::RobotArm(std::atomic<float>& angle, std::atomic<bool>& newInputReceived, float scale)
    : shader("res/shaders/basic.shader"), angle(angle), newInputReceived(newInputReceived), scale(scale),
    appendage_1(scale), appendage_2(scale) {
}

RobotArm::~RobotArm() {}

void RobotArm::Initialize(float posX, float posY, float initialRotationDegrees) {
    this->posX = posX;
    this->posY = posY;
    this->initialRotationRadians = initialRotationDegrees * (M_PI / 180.0f);

    appendage_1.Initialize();
    appendage_2.Initialize();

    shader.Bind();
}

void RobotArm::Update() {
    // Update positions with rotation based on the current angle for the first appendage
    appendage_1.UpdateRotation(angle, 0.0f, 0.0f);

    if (newInputReceived) {
        // Calculate the red dot position of the first appendage
        auto redDotPosition_1 = appendage_1.CalculateRedDotPosition();

        // Translate the second appendage to align its red dot with the red dot of the first appendage
        appendage_2.TranslateToPosition(redDotPosition_1.first, redDotPosition_1.second);

        std::cout << "New input received. Angle: " << angle << std::endl;
        newInputReceived = false; // Reset the flag
    }
}

void RobotArm::Render() {
    shader.Bind(); // Ensure shader is bound before rendering

    int location = shader.GetUniformLocation("u_Color");

    // Draw appendage 1
    appendage_1.Render(shader);

    // Draw appendage 2
    appendage_2.Render(shader);

    // Calculate red dot positions
    auto redDotPosition_1 = appendage_1.CalculateRedDotPosition();
    auto redDotPosition_2 = appendage_2.CalculateRedDotPosition();

    // Render red dots at the positions
    RenderDot(redDotPosition_1.first, redDotPosition_1.second, 0.01f, 1.0f, 0.0f, 0.0f);
    RenderDot(redDotPosition_2.first, redDotPosition_2.second, 0.01f, 1.0f, 0.0f, 0.0f);
}

void RobotArm::RenderDot(float x, float y, float size, float r, float g, float b) {
    int location = shader.GetUniformLocation("u_Color");
    GLCall(glUniform4f(location, r, g, b, 1.0f)); // Set color

    float dotVertices[] = {
        x - size, y - size,
        x + size, y - size,
        x + size, y + size,
        x - size, y + size
    };

    unsigned int dotIndices[] = {
        0, 1, 2,
        2, 3, 0
    };

    unsigned int vao, vbo, ebo;
    GLCall(glGenVertexArrays(1, &vao));
    GLCall(glGenBuffers(1, &vbo));
    GLCall(glGenBuffers(1, &ebo));

    GLCall(glBindVertexArray(vao));

    GLCall(glBindBuffer(GL_ARRAY_BUFFER, vbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(dotVertices), dotVertices, GL_STATIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(dotIndices), dotIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    GLCall(glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, nullptr));

    GLCall(glDeleteVertexArrays(1, &vao));
    GLCall(glDeleteBuffers(1, &vbo));
    GLCall(glDeleteBuffers(1, &ebo));
}
