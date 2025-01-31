// File: src/RobotArm.cpp
#include "RobotArm.h"
#include <iostream>
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArm::RobotArm(std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<bool>& newInputReceived, float scale)
    : shader("res/shaders/basic.shader"), angle1(angle1), angle2(angle2), newInputReceived(newInputReceived), scale(scale),
    appendage_1(scale), appendage_2(scale), endEffector(scale), autoUpdateEnabled(false), autoAngle1(-25.0f * (M_PI / 180.0f)), autoAngle2(25.0f * (M_PI / 180.0f)), autoAngle3(90.0f * (M_PI / 180.0f)), autoDirection(1), currentPhase(0), animationComplete(false) {
}

RobotArm::~RobotArm() {}

void RobotArm::Initialize(float posX, float posY, float initialRotationDegrees) {
    this->posX = posX;
    this->posY = posY;
    this->initialRotationRadians = initialRotationDegrees * (M_PI / 180.0f);

    appendage_1.Initialize();
    appendage_2.Initialize();
    endEffector.Initialize(); // Initialize the end effector

    shader.Bind();
}

void RobotArm::StartAutoSimulation() {
    autoUpdateEnabled = true;
}

void RobotArm::Update() {
    // Commented out manual update code
    /*
    if (newInputReceived) {
        // Update positions with rotation based on the current angle for the first appendage
        appendage_1.UpdateRotation(angle1, 0.0f, 0.0f);
        // Calculate the red dot position of the first appendage
        auto redDotPosition_1 = appendage_1.CalculateRedDotPosition();
        appendage_2.UpdateRotation(angle2, redDotPosition_1.first, redDotPosition_1.second);

        // Translate the second appendage to align its red dot with the red dot of the first appendage
        appendage_2.TranslateToPosition(redDotPosition_1.first, redDotPosition_1.second);

        // Convert angles from radians to degrees
        float angle1Degrees = angle1 * (180.0f / M_PI);
        float angle2Degrees = angle2 * (180.0f / M_PI);

        std::cout << "New input received. Angle1: " << angle1 << " radians (" << angle1Degrees << " degrees), "
            << "Angle2: " << angle2 << " radians (" << angle2Degrees << " degrees)" << std::endl;

        newInputReceived = false; // Reset the flag
    }
    */

    if (animationComplete || !autoUpdateEnabled) {
        return; // Stop updating if the animation is complete or auto-update is not enabled
    }

    // Auto-update simulation
    const float increment = 0.01f; // Faster increment value for smooth movement
    const float angles[][3] = {
        {-25.0f, 25.0f, 90.0f},
        {25.0f, -25.0f, 90.0f},
        {-25.0f, 25.0f, 90.0f},
        {155.0f, 205.0f, 90.0f},
        {205.0f, 155.0f, 270.0f},
        {155.0f, 205.0f, 270.0f},
        {205.0f, 155.0f, 270.0f},
        {155.0f, 205.0f, 270.0f},
        {-25.0f, 25.0f, 90.0f},
        {25.0f, -25.0f, 90.0f},
        {-25.0f, 25.0f, 90.0f}
    };
    const int numPhases = sizeof(angles) / sizeof(angles[0]);

    // Convert current phase angles to radians
    float targetAngle1 = angles[currentPhase][0] * (M_PI / 180.0f);
    float targetAngle2 = angles[currentPhase][1] * (M_PI / 180.0f);
    float targetAngle3 = angles[currentPhase][2] * (M_PI / 180.0f);
    // Update auto angles
    if (currentPhase == 3) {
        // Incremental movement from -25, 25 to 155, 205
        autoAngle1 += autoDirection * increment;
        autoAngle2 += autoDirection * increment;
        autoAngle3 += autoDirection * increment;
        if (autoAngle1 >= targetAngle1 && autoAngle2 >= targetAngle2 && autoAngle3 >= targetAngle3) {
            currentPhase++;
        }
    }
    else {
        if (std::abs(targetAngle1 - autoAngle1) > 0.0001f) {
            autoAngle1 += autoDirection * increment * (targetAngle1 - autoAngle1) / std::abs(targetAngle1 - autoAngle1);
        }
        if (std::abs(targetAngle2 - autoAngle2) > 0.0001f) {
            autoAngle2 += autoDirection * increment * (targetAngle2 - autoAngle2) / std::abs(targetAngle2 - autoAngle2);
        }
        if (std::abs(targetAngle3 - autoAngle3) > 0.0001f) {
            autoAngle3 += autoDirection * increment * (targetAngle3 - autoAngle3) / std::abs(targetAngle3 - autoAngle3);
        }
        if (std::abs(autoAngle1 - targetAngle1) < increment && std::abs(autoAngle2 - targetAngle2) < increment && std::abs(autoAngle3 - targetAngle3) < increment) {
            currentPhase++;
        }
    }

    // Check if all phases are completed
    if (currentPhase >= numPhases) {
        animationComplete = true; // Mark the animation as complete
    }

    // Debugging output
    float autoAngle1Degrees = autoAngle1 * (180.0f / M_PI);
    float autoAngle2Degrees = autoAngle2 * (180.0f / M_PI);
    float autoAngle3Degrees = autoAngle3 * (180.0f / M_PI);

    // Update positions with rotation based on the current auto angles
    appendage_1.UpdateRotation(autoAngle1, 0.0f, 0.0f);
    auto redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
    appendage_2.UpdateRotation(autoAngle2, redDotPosition_1.first, redDotPosition_1.second);
    appendage_2.TranslateToPosition(redDotPosition_1.first, redDotPosition_1.second);

    // Update the end effector position
    auto redDotPosition_2 = appendage_2.CalculateRedDotPosition("left");
    endEffector.UpdateRotation(autoAngle3, redDotPosition_2.first, redDotPosition_2.second);
    endEffector.TranslateToPosition(redDotPosition_2.first, redDotPosition_2.second);
}

void RobotArm::Render() {
    shader.Bind(); // Ensure shader is bound before rendering

    int location = shader.GetUniformLocation("u_Color");

    // Draw appendage 1
    appendage_1.Render(shader);

    // Draw appendage 2
    appendage_2.Render(shader);

    // Draw end effector
    endEffector.Render(shader);

    // Calculate red dot positions
    auto redDotPosition_1 = appendage_1.CalculateRedDotPosition("right");
    auto redDotPosition_2 = appendage_2.CalculateRedDotPosition("right");

    // Render red dots at the positions
    //RenderDot(redDotPosition_1.first, redDotPosition_1.second, 0.01f, 1.0f, 0.0f, 0.0f);
    //RenderDot(redDotPosition_2.first, redDotPosition_2.second, 0.01f, 1.0f, 0.0f, 0.0f);
}

void RobotArm::RenderDot(float x, float y, float size, float r, float g, float b) {
    int location = shader.GetUniformLocation("u_Color");
    GLCall(glUniform4f(location, r, g, b, 1.0f)); // Set color

    float dotVertices[] = {
        x - size, y - size,
        x + size, y - size,
        x + size, y + size,
        x - size, y + size
    };

    unsigned int dotIndices[] = {
        0, 1, 2,
        2, 3, 0
    };

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
