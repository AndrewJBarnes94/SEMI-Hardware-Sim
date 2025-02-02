#pragma once

#include "Shader.h"
#include <utility>

class RobotArmEndEffector {
public:
    RobotArmEndEffector(float scale);
    ~RobotArmEndEffector();

    void Initialize();
    void UpdateRotation(float angle, float centerX, float centerY);
    void TranslateToPosition(float x, float y);
    void Render(const Shader& shader);
    std::pair<float, float> CalculateRedDotPosition() const;

private:
    void TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY);
    std::pair<float, float> CalculateRectangleHeightMidpoint() const;
    void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);

    float scale;
    int numCircleVertices;
    int numRectangleVertices;
    int numCircleIndices;
    int numRectangleIndices;
    float circlePositions[42]; // 21 vertices * 2 (x, y)
    float initialCirclePositions[42];
    unsigned int circleIndices[60]; // 20 triangles * 3 indices
    float rectanglePositions[8]; // 4 vertices * 2 (x, y)
    float initialRectanglePositions[8];
    unsigned int rectangleIndices[6]; // 2 triangles * 3 indices
    unsigned int circleVao, circleVbo, circleEbo;
    unsigned int rectangleVao, rectangleVbo, rectangleEbo;
};

