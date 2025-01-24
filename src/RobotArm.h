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
    struct Appendage {
        unsigned int circleVao, circleVbo, circleEbo;
        unsigned int rectangleVao, rectangleVbo, rectangleEbo;
        float circlePositions[84]; // 2 * (20 + 1) * 2
        float rectanglePositions[8]; // 4 * 2
        float initialCirclePositions[84]; // Store initial positions
        float initialRectanglePositions[8]; // Store initial positions
        unsigned int circleIndices[120]; // 2 * 3 * 20
        unsigned int rectangleIndices[6]; // 6
        int numCircleVertices;
        int numRectangleVertices;
        int numCircleIndices;
        int numRectangleIndices;
    };

    void InitializeAppendage(Appendage& appendage);
    void UpdateRotation(Appendage& appendage, float angle, float centerX, float centerY);
    void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);
    void TranslateToPivot(float* positions, int numVertices, float pivotX, float pivotY);
    void RenderAppendage(const Appendage& appendage);
    void RenderDot(float x, float y, float size, float r, float g, float b);

    std::pair<float, float> CalculateHalfCircleMidwayPoint(const Appendage& appendage) const;
    std::pair<float, float> CalculateRectangleHeightMidpoint(const Appendage& appendage) const;

    Shader shader;
    Appendage appendage_1;
    Appendage appendage_2;
    std::atomic<float>& rotationSpeed;
    std::atomic<bool>& newInputReceived;
    float scale;
    float posX, posY;
    float initialRotationRadians;
};

#endif // ROBOT_ARM_H
