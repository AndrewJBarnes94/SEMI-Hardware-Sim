// File: RobotArm.h
#ifndef ROBOT_ARM_H
#define ROBOT_ARM_H

#include <atomic>
#include "Shader.h"
#include "RobotArmAppendage.h"
#include "RobotArmEndEffector.h"

class RobotArm {
public:
    RobotArm(std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<bool>& newInputReceived, float scale = 0.5f);
    ~RobotArm();
    void Initialize(float posX, float posY, float initialRotationDegrees);
    void Update();
    void Render();
    void RenderDot(float x, float y, float size, float r, float g, float b);
    void SetAngles(float angle1, float angle2);

private:
    Shader shader;
    RobotArmAppendage appendage_1;
    RobotArmAppendage appendage_2;
    RobotArmEndEffector endEffector;
    std::atomic<float>& angle1;
    std::atomic<float>& angle2;
    std::atomic<bool>& newInputReceived;
    float scale;
    float posX, posY;
    float initialRotationRadians;
};

#endif // ROBOT_ARM_H
