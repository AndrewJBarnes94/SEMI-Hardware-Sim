#include "RobotArm.h"
#include <iostream>
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArm::RobotArm(std::atomic<float>& rotationSpeed, std::atomic<bool>& newInputReceived, float scale)
    : shader("res/shaders/basic.shader"), rotationSpeed(rotationSpeed), newInputReceived(newInputReceived), scale(scale) {
    appendage_1.numCircleVertices = 2 * (20 + 1);
    appendage_1.numRectangleVertices = 4;
    appendage_1.numCircleIndices = 2 * 3 * 20;
    appendage_1.numRectangleIndices = 6;

    appendage_2.numCircleVertices = 2 * (20 + 1);
    appendage_2.numRectangleVertices = 4;
    appendage_2.numCircleIndices = 2 * 3 * 20;
    appendage_2.numRectangleIndices = 6;
}

RobotArm::~RobotArm() {
    GLCall(glDeleteVertexArrays(1, &appendage_1.circleVao));
    GLCall(glDeleteBuffers(1, &appendage_1.circleVbo));
    GLCall(glDeleteBuffers(1, &appendage_1.circleEbo));
    GLCall(glDeleteVertexArrays(1, &appendage_1.rectangleVao));
    GLCall(glDeleteBuffers(1, &appendage_1.rectangleVbo));
    GLCall(glDeleteBuffers(1, &appendage_1.rectangleEbo));

    GLCall(glDeleteVertexArrays(1, &appendage_2.circleVao));
    GLCall(glDeleteBuffers(1, &appendage_2.circleVbo));
    GLCall(glDeleteBuffers(1, &appendage_2.circleEbo));
    GLCall(glDeleteVertexArrays(1, &appendage_2.rectangleVao));
    GLCall(glDeleteBuffers(1, &appendage_2.rectangleVbo));
    GLCall(glDeleteBuffers(1, &appendage_2.rectangleEbo));
}

void RobotArm::Initialize(float posX, float posY, float initialRotationDegrees) {
    this->posX = posX;
    this->posY = posY;
    this->initialRotationRadians = initialRotationDegrees * (M_PI / 180.0f);

    InitializeAppendage(appendage_1);
    InitializeAppendage(appendage_2);

    // Store initial positions
    std::copy(std::begin(appendage_1.circlePositions), std::end(appendage_1.circlePositions), std::begin(appendage_1.initialCirclePositions));
    std::copy(std::begin(appendage_1.rectanglePositions), std::end(appendage_1.rectanglePositions), std::begin(appendage_1.initialRectanglePositions));

    std::copy(std::begin(appendage_2.circlePositions), std::end(appendage_2.circlePositions), std::begin(appendage_2.initialCirclePositions));
    std::copy(std::begin(appendage_2.rectanglePositions), std::end(appendage_2.rectanglePositions), std::begin(appendage_2.initialRectanglePositions));

    // Update positions with initial rotation
    UpdateRotation(appendage_1, initialRotationRadians, 0.0f, 0.0f);
    UpdateRotation(appendage_2, initialRotationRadians, 0.5f, 0.5f);

    shader.Bind();
}

void RobotArm::InitializeAppendage(Appendage& appendage) {
    int index = 0;

    // Left half-circle vertices (from -?/2 to ?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = -M_PI / 2 + M_PI * i / 20;
        appendage.circlePositions[index++] = scale * (0.4f + 0.2f * cos(theta));
        appendage.circlePositions[index++] = scale * (0.2f * sin(theta));
    }

    // Right half-circle vertices (from ?/2 to 3?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = M_PI / 2 + M_PI * i / 20;
        appendage.circlePositions[index++] = scale * (-0.4f + 0.2f * cos(theta));
        appendage.circlePositions[index++] = scale * (0.2f * sin(theta));
    }

    // Rectangle vertices
    appendage.rectanglePositions[0] = scale * 0.4f;  // Top right
    appendage.rectanglePositions[1] = scale * 0.2f;
    appendage.rectanglePositions[2] = scale * 0.4f;  // Bottom right
    appendage.rectanglePositions[3] = scale * -0.2f;
    appendage.rectanglePositions[4] = scale * -0.4f; // Bottom left
    appendage.rectanglePositions[5] = scale * -0.2f;
    appendage.rectanglePositions[6] = scale * -0.4f; // Top left
    appendage.rectanglePositions[7] = scale * 0.2f;

    // Translate the entire geometry so that the center of the left half-circle is at the origin
    TranslateToCenter(appendage.circlePositions, appendage.numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(appendage.rectanglePositions, appendage.numRectangleVertices, -scale * 0.4f, 0.0f);

    index = 0;

    // Left half-circle indices
    for (int i = 0; i < 20; ++i) {
        appendage.circleIndices[index++] = 0;
        appendage.circleIndices[index++] = i;
        appendage.circleIndices[index++] = i + 1;
    }

    // Right half-circle indices
    for (int i = 21; i < 41; ++i) {
        appendage.circleIndices[index++] = 21;
        appendage.circleIndices[index++] = i;
        appendage.circleIndices[index++] = i + 1;
    }

    // Rectangle indices
    appendage.rectangleIndices[0] = 0; // Top right
    appendage.rectangleIndices[1] = 1; // Bottom right
    appendage.rectangleIndices[2] = 2; // Bottom left
    appendage.rectangleIndices[3] = 0; // Top right
    appendage.rectangleIndices[4] = 2; // Bottom left
    appendage.rectangleIndices[5] = 3; // Top left

    // Vertex Array Object and Buffer for circles
    GLCall(glGenVertexArrays(1, &appendage.circleVao));
    GLCall(glGenBuffers(1, &appendage.circleVbo));
    GLCall(glGenBuffers(1, &appendage.circleEbo));

    // Initialize VAO and VBO for circles
    GLCall(glBindVertexArray(appendage.circleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage.circleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * appendage.numCircleVertices * 2, appendage.circlePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, appendage.circleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * appendage.numCircleIndices, appendage.circleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Vertex Array Object and Buffer for rectangle
    GLCall(glGenVertexArrays(1, &appendage.rectangleVao));
    GLCall(glGenBuffers(1, &appendage.rectangleVbo));
    GLCall(glGenBuffers(1, &appendage.rectangleEbo));

    // Initialize VAO and VBO for rectangle
    GLCall(glBindVertexArray(appendage.rectangleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage.rectangleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * appendage.numRectangleVertices * 2, appendage.rectanglePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, appendage.rectangleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * appendage.numRectangleIndices, appendage.rectangleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));
}

void RobotArm::Update() {
    if (newInputReceived) {
        // Update positions with rotation based on new input
        UpdateRotation(appendage_1, rotationSpeed, 0.0f, 0.0f);
        UpdateRotation(appendage_2, rotationSpeed, 0.5f, 0.5f);

        // Update VBOs
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_1.circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_1.numCircleVertices * 2, appendage_1.circlePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_1.rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_1.numRectangleVertices * 2, appendage_1.rectanglePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_2.circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_2.numCircleVertices * 2, appendage_2.circlePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_2.rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_2.numRectangleVertices * 2, appendage_2.rectanglePositions));

        PrintCenterPoints();

        newInputReceived = false; // Reset the flag
    }
}

void RobotArm::PrintCenterPoints() const {
    // Calculate the center point of the circle for appendage_1
    float centerX1 = 0.0f;
    float centerY1 = 0.0f;
    for (int i = 0; i <= 20; ++i) {
        centerX1 += appendage_1.circlePositions[i * 2];
        centerY1 += appendage_1.circlePositions[i * 2 + 1];
    }
    centerX1 /= 21;
    centerY1 /= 21;

    // Calculate the center point of the circle for appendage_2
    float centerX2 = 0.0f;
    float centerY2 = 0.0f;
    for (int i = 0; i <= 20; ++i) {
        centerX2 += appendage_2.circlePositions[i * 2];
        centerY2 += appendage_2.circlePositions[i * 2 + 1];
    }
    centerX2 /= 21;
    centerY2 /= 21;

    std::cout << "Appendage 1 Circle Center Point: (" << centerX1 << ", " << centerY1 << ")" << std::endl;
    std::cout << "Appendage 2 Circle Center Point: (" << centerX2 << ", " << centerY2 << ")" << std::endl;
}

void RobotArm::Render() {
    int location = shader.GetUniformLocation("u_Color");

    // Draw appendage 1
    RenderAppendage(appendage_1);

    // Draw appendage 2
    RenderAppendage(appendage_2);
}

void RobotArm::UpdateRotation(Appendage& appendage, float angle, float centerX, float centerY) {
    for (int i = 0; i < appendage.numCircleVertices * 2; i += 2) {
        float x = appendage.initialCirclePositions[i] - centerX;
        float y = appendage.initialCirclePositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        appendage.circlePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        appendage.circlePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }

    for (int i = 0; i < appendage.numRectangleVertices * 2; i += 2) {
        float x = appendage.initialRectanglePositions[i] - centerX;
        float y = appendage.initialRectanglePositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        appendage.rectanglePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        appendage.rectanglePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }
}

void RobotArm::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

void RobotArm::RenderAppendage(const Appendage& appendage) {
    int location = shader.GetUniformLocation("u_Color");

    // Draw the outline
    GLCall(glLineWidth(2.0f)); // Set the line width for the outline
    GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f)); // Set color to black

    // Draw the circles outline
    GLCall(glBindVertexArray(appendage.circleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, appendage.numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle outline
    GLCall(glBindVertexArray(appendage.rectangleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, appendage.numRectangleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the fill
    GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver

    // Draw the circles fill
    GLCall(glBindVertexArray(appendage.circleVao));
    GLCall(glDrawElements(GL_TRIANGLES, appendage.numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle fill
    GLCall(glBindVertexArray(appendage.rectangleVao));
    GLCall(glDrawElements(GL_TRIANGLES, appendage.numRectangleIndices, GL_UNSIGNED_INT, nullptr));
}
