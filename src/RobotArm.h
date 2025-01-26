// File: src/RobotArm.h
#ifndef ROBOT_ARM_H
#define ROBOT_ARM_H

#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <atomic>
#include "Shader.h"
#include "RobotArmAppendage.h"

class RobotArm {
public:
    RobotArm(std::atomic<float>& angle, std::atomic<bool>& newInputReceived, float scale = 0.5f);
    ~RobotArm();

    void Initialize(float posX, float posY, float initialRotationDegrees);
    void Update();
    void Render();

private:
    void RenderDot(float x, float y, float size, float r, float g, float b);

    Shader shader;
    RobotArmAppendage appendage_1;
    RobotArmAppendage appendage_2;
    std::atomic<bool>& newInputReceived;
    std::atomic<float>& angle;
    float scale;
    float posX, posY;
    float initialRotationRadians;
};

#endif // ROBOT_ARM_H
