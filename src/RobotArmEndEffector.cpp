#include "RobotArmEndEffector.h"
#include <GL/glew.h>
#include <cmath>

EndEffector::EndEffector(std::atomic<float>& angle, std::atomic<bool>& newInputReceived, float scale)
    : angle(angle), newInputReceived(newInputReceived), scale(scale), x(0.0f), y(0.0f), rotation(0.0f) {
    SetupGeometry();
}

void EndEffector::Initialize(float x, float y, float rotation) {
    this->x = x;
    this->y = y;
    this->rotation = rotation;
}

void EndEffector::Update() {
    if (newInputReceived) {
        rotation = angle.load();
        newInputReceived = false;
    }
}

void EndEffector::Render() {
    glBindVertexArray(VAO);
    // Apply transformations and render the end effector
    // (Transformation code would go here)
    glDrawArrays(GL_TRIANGLE_FAN, 0, 18); // Adjust the number of vertices as needed
    glBindVertexArray(0);
}

void EndEffector::SetupGeometry() {
    float vertices[] = {
        // Rectangle part
        -0.5f * scale, -0.1f * scale, 0.0f,
         0.5f * scale, -0.1f * scale, 0.0f,
         0.5f * scale,  0.1f * scale, 0.0f,
        -0.5f * scale,  0.1f * scale, 0.0f,

        // Half circle part (left side)
        -0.5f * scale,  0.1f * scale, 0.0f,
        -0.5f * scale, -0.1f * scale, 0.0f,
        -0.6f * scale, -0.1f * scale, 0.0f,
        -0.6f * scale,  0.1f * scale, 0.0f,
        -0.5f * scale,  0.1f * scale, 0.0f,

        // Fork part (right side)
         0.5f * scale,  0.1f * scale, 0.0f,
         0.6f * scale,  0.1f * scale, 0.0f,
         0.6f * scale,  0.05f * scale, 0.0f,
         0.55f * scale,  0.05f * scale, 0.0f,
         0.55f * scale, -0.05f * scale, 0.0f,
         0.6f * scale, -0.05f * scale, 0.0f,
         0.6f * scale, -0.1f * scale, 0.0f,
         0.5f * scale, -0.1f * scale, 0.0f
    };

    glGenVertexArrays(1, &VAO);
    glGenBuffers(1, &VBO);

    glBindVertexArray(VAO);

    glBindBuffer(GL_ARRAY_BUFFER, VBO);
    glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 3 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindVertexArray(0);
}
