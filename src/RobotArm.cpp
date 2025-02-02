// File: RobotArm.cpp
#include "RobotArm.h"
#include <iostream>
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArm::RobotArm(std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<float>& angle3, std::atomic<bool>& newInputReceived, float scale)
    : shader("res/shaders/basic.shader"), angle1(angle1), angle2(angle2), angle3(angle3), newInputReceived(newInputReceived), scale(scale),
    appendage_1(scale), appendage_2(scale), endEffector(scale) {
}

RobotArm::~RobotArm() {}

void RobotArm::Initialize(float posX, float posY, float initialRotationDegrees) {
    this->posX = posX;
    this->posY = posY;
    this->initialRotationRadians = initialRotationDegrees * (M_PI / 180.0f);

    appendage_1.Initialize();
    appendage_2.Initialize();
    endEffector.Initialize();

    shader.Bind();
}

void RobotArm::Update() {
    std::cout << "Updating robot arm with angles: angle1 = " << angle1 << ", angle2 = " << angle2 << ", angle3 = " << angle3 << std::endl;

    // Convert angles to radians
    float angle1Rad = angle1 * (M_PI / 180.0f);
    float angle2Rad = angle2 * (M_PI / 180.0f);
    float angle3Rad = angle3 * (M_PI / 180.0f);

    appendage_1.UpdateRotation(angle1Rad, 0.0f, 0.0f);
    auto redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
    appendage_2.UpdateRotation(angle2Rad, redDotPosition_1.first, redDotPosition_1.second);
    appendage_2.TranslateToPosition(redDotPosition_1.first, redDotPosition_1.second);

    auto redDotPosition_2 = appendage_2.CalculateRedDotPosition("left");
    endEffector.UpdateRotation(angle3Rad, redDotPosition_2.first, redDotPosition_2.second);
    endEffector.TranslateToPosition(redDotPosition_2.first, redDotPosition_2.second);

    // Reset newInputReceived to false after processing the update
    newInputReceived = false;
}

void RobotArm::Render() {
    shader.Bind();

    appendage_1.Render(shader);
    appendage_2.Render(shader);
    endEffector.Render(shader);

    auto redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
    auto redDotPosition_2 = appendage_2.CalculateRedDotPosition("left");

    RenderDot(redDotPosition_1.first, redDotPosition_1.second, 0.01f, 1.0f, 0.0f, 0.0f);
    RenderDot(redDotPosition_2.first, redDotPosition_2.second, 0.01f, 1.0f, 0.0f, 0.0f);
}

void RobotArm::RenderDot(float x, float y, float size, float r, float g, float b) {
    int location = shader.GetUniformLocation("u_Color");
    GLCall(glUniform4f(location, r, g, b, 1.0f));

    float dotVertices[] = {
        x - size, y - size,
        x + size, y - size,
        x + size, y + size,
        x - size, y + size
    };

    unsigned int dotIndices[] = { 0, 1, 2, 2, 3, 0 };

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

void RobotArm::SetAngles(float angle1, float angle2, float angle3) {
    this->angle1 = angle1;
    this->angle2 = angle2;
    this->angle3 = angle3;
    newInputReceived = true;
    std::cout << "SetAngles called: angle1 = " << angle1 << ", angle2 = " << angle2 << ", angle3 = " << angle3 << std::endl;
}
