#ifndef ROBOT_ARM_H
#define ROBOT_ARM_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <atomic>
#include "Shader.h"

class RobotArm {
public:
    RobotArm(std::atomic<float>& rotationSpeed, std::atomic<bool>& newInputReceived, float scale = 0.5f);
    ~RobotArm();

    void Initialize(float posX, float posY, float initialRotationDegrees);
    void Update();
    void Render();

private:
    void InitializeAppendage(unsigned int& circleVao, unsigned int& circleVbo, unsigned int& circleEbo,
        unsigned int& rectangleVao, unsigned int& rectangleVbo, unsigned int& rectangleEbo,
        float* circlePositions, float* rectanglePositions, unsigned int* circleIndices, unsigned int* rectangleIndices,
        int& numCircleVertices, int& numRectangleVertices, int& numCircleIndices, int& numRectangleIndices);
    void UpdateRotation(float* positions, const float* initialPositions, int numVertices, float angle, float centerX, float centerY);
    void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);
    void RenderAppendage(unsigned int circleVao, unsigned int rectangleVao, int numCircleIndices, int numRectangleIndices);

    Shader shader;
    unsigned int appendage_1_circleVao, appendage_1_circleVbo, appendage_1_circleEbo;
    unsigned int appendage_1_rectangleVao, appendage_1_rectangleVbo, appendage_1_rectangleEbo;
    float appendage_1_circlePositions[84]; // 2 * (20 + 1) * 2
    float appendage_1_rectanglePositions[8]; // 4 * 2
    float appendage_1_initialCirclePositions[84]; // Store initial positions
    float appendage_1_initialRectanglePositions[8]; // Store initial positions
    unsigned int appendage_1_circleIndices[120]; // 2 * 3 * 20
    unsigned int appendage_1_rectangleIndices[6]; // 6
    int appendage_1_numCircleVertices;
    int appendage_1_numRectangleVertices;
    int appendage_1_numCircleIndices;
    int appendage_1_numRectangleIndices;

    unsigned int appendage_2_circleVao, appendage_2_circleVbo, appendage_2_circleEbo;
    unsigned int appendage_2_rectangleVao, appendage_2_rectangleVbo, appendage_2_rectangleEbo;
    float appendage_2_circlePositions[84]; // 2 * (20 + 1) * 2
    float appendage_2_rectanglePositions[8]; // 4 * 2
    float appendage_2_initialCirclePositions[84]; // Store initial positions
    float appendage_2_initialRectanglePositions[8]; // Store initial positions
    unsigned int appendage_2_circleIndices[120]; // 2 * 3 * 20
    unsigned int appendage_2_rectangleIndices[6]; // 6
    int appendage_2_numCircleVertices;
    int appendage_2_numRectangleVertices;
    int appendage_2_numCircleIndices;
    int appendage_2_numRectangleIndices;

    std::atomic<float>& rotationSpeed;
    std::atomic<bool>& newInputReceived;
    float scale;
    float posX, posY;
    float initialRotationRadians;
};

#endif // ROBOT_ARM_H




