// File: RobotArm.h
#pragma once

#include <atomic>
#include "Shader.h"
#include "RobotArmAppendage.h"
#include "RobotArmEndEffector.h"

class RobotArm {
public:
    RobotArm(std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<float>& angle3, std::atomic<bool>& newInputReceived, float scale);
    ~RobotArm();

    void Initialize(float posX, float posY, float initialRotationDegrees);
    void Update();
    void Render();
    void SetAngles(float angle1, float angle2, float angle3);

private:
    void RenderDot(float x, float y, float size, float r, float g, float b);

    Shader shader;
    std::atomic<float>& angle1;
    std::atomic<float>& angle2;
    std::atomic<float>& angle3;
    std::atomic<bool>& newInputReceived;
    float scale;
    float posX;
    float posY;
    float initialRotationRadians;
    RobotArmAppendage appendage_1; 
    RobotArmAppendage appendage_2;
    RobotArmEndEffector endEffector;
};