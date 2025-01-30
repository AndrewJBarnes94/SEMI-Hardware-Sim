// File: src/RobotArmEndEffector.h

#ifndef ROBOT_ARM_END_EFFECTOR_H
#define ROBOT_ARM_END_EFFECTOR_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include "Shader.h"

class RobotArmEndEffector {
public:
    RobotArmEndEffector(float scale);
    ~RobotArmEndEffector();
    void Initialize();
    void UpdateRotation(float angle, float centerX, float centerY);
    void Render(const Shader& shader);
    std::pair<float, float> CalculateRectangleHeightMidpoint() const;
    void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);
    void TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY);
    void TranslateToPosition(float x, float y);
    std::pair<float, float> CalculateRedDotPosition() const; // Add this line

    unsigned int circleVao, circleVbo, circleEbo;
    unsigned int rectangleVao, rectangleVbo, rectangleEbo;
    float circlePositions[84]; // 2 * (20 + 1) * 2
    float rectanglePositions[8]; // 4 * 2
    float initialCirclePositions[84]; // Store initial positions
    float initialRectanglePositions[8]; // Store initial positions
    unsigned int circleIndices[120]; // 2 * 3 * 20
    unsigned int rectangleIndices[6];
    int numCircleVertices;
    int numRectangleVertices;
    int numCircleIndices; // Add this line
    int numRectangleIndices; // Add this line

private:
    float scale;
};

#endif // ROBOT_ARM_END_EFFECTOR_H
