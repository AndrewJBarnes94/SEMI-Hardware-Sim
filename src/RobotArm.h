#ifndef ROBOT_ARM_H
#define ROBOT_ARM_H

#include <atomic>
#include "Shader.h"
#include "RobotArmAppendage.h"

class RobotArm {
public:
    RobotArm(std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<bool>& newInputReceived, float scale = 0.5f);
    ~RobotArm();
    void Initialize(float posX, float posY, float initialRotationDegrees);
    void Update();
    void Render();
    void RenderDot(float x, float y, float size, float r, float g, float b);

private:
    Shader shader;
    RobotArmAppendage appendage_1;
    RobotArmAppendage appendage_2;
    std::atomic<float>& angle1;
    std::atomic<float>& angle2;
    std::atomic<bool>& newInputReceived;
    float scale;
    float posX, posY;
    float initialRotationRadians;

    // Auto-update variables
    bool autoUpdateEnabled;
    float autoAngle1;
    float autoAngle2;
    int autoDirection;
    int currentPhase;
};

#endif // ROBOT_ARM_H



