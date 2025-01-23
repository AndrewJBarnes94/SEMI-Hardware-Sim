#include "RobotArm.h"
#include <iostream>
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArm::RobotArm(std::atomic<float>& rotationSpeed, std::atomic<bool>& newInputReceived, float scale)
    : shader("res/shaders/basic.shader"), rotationSpeed(rotationSpeed), newInputReceived(newInputReceived), scale(scale) {
    appendage_1_numCircleVertices = 2 * (20 + 1);
    appendage_1_numRectangleVertices = 4;
    appendage_1_numCircleIndices = 2 * 3 * 20;
    appendage_1_numRectangleIndices = 6;

    appendage_2_numCircleVertices = 2 * (20 + 1);
    appendage_2_numRectangleVertices = 4;
    appendage_2_numCircleIndices = 2 * 3 * 20;
    appendage_2_numRectangleIndices = 6;
}

RobotArm::~RobotArm() {
    GLCall(glDeleteVertexArrays(1, &appendage_1_circleVao));
    GLCall(glDeleteBuffers(1, &appendage_1_circleVbo));
    GLCall(glDeleteBuffers(1, &appendage_1_circleEbo));
    GLCall(glDeleteVertexArrays(1, &appendage_1_rectangleVao));
    GLCall(glDeleteBuffers(1, &appendage_1_rectangleVbo));
    GLCall(glDeleteBuffers(1, &appendage_1_rectangleEbo));

    GLCall(glDeleteVertexArrays(1, &appendage_2_circleVao));
    GLCall(glDeleteBuffers(1, &appendage_2_circleVbo));
    GLCall(glDeleteBuffers(1, &appendage_2_circleEbo));
    GLCall(glDeleteVertexArrays(1, &appendage_2_rectangleVao));
    GLCall(glDeleteBuffers(1, &appendage_2_rectangleVbo));
    GLCall(glDeleteBuffers(1, &appendage_2_rectangleEbo));
}

void RobotArm::Initialize(float posX, float posY, float initialRotationDegrees) {
    this->posX = posX;
    this->posY = posY;
    this->initialRotationRadians = initialRotationDegrees * (M_PI / 180.0f);

    InitializeAppendage(appendage_1_circleVao, appendage_1_circleVbo, appendage_1_circleEbo,
        appendage_1_rectangleVao, appendage_1_rectangleVbo, appendage_1_rectangleEbo,
        appendage_1_circlePositions, appendage_1_rectanglePositions, appendage_1_circleIndices, appendage_1_rectangleIndices,
        appendage_1_numCircleVertices, appendage_1_numRectangleVertices, appendage_1_numCircleIndices, appendage_1_numRectangleIndices);

    InitializeAppendage(appendage_2_circleVao, appendage_2_circleVbo, appendage_2_circleEbo,
        appendage_2_rectangleVao, appendage_2_rectangleVbo, appendage_2_rectangleEbo,
        appendage_2_circlePositions, appendage_2_rectanglePositions, appendage_2_circleIndices, appendage_2_rectangleIndices,
        appendage_2_numCircleVertices, appendage_2_numRectangleVertices, appendage_2_numCircleIndices, appendage_2_numRectangleIndices);

    // Store initial positions
    std::copy(std::begin(appendage_1_circlePositions), std::end(appendage_1_circlePositions), std::begin(appendage_1_initialCirclePositions));
    std::copy(std::begin(appendage_1_rectanglePositions), std::end(appendage_1_rectanglePositions), std::begin(appendage_1_initialRectanglePositions));

    std::copy(std::begin(appendage_2_circlePositions), std::end(appendage_2_circlePositions), std::begin(appendage_2_initialCirclePositions));
    std::copy(std::begin(appendage_2_rectanglePositions), std::end(appendage_2_rectanglePositions), std::begin(appendage_2_initialRectanglePositions));

    // Update positions with initial rotation
    UpdateRotation(appendage_1_circlePositions, appendage_1_initialCirclePositions, appendage_1_numCircleVertices, initialRotationRadians, 0.0f, 0.0f);
    UpdateRotation(appendage_1_rectanglePositions, appendage_1_initialRectanglePositions, appendage_1_numRectangleVertices, initialRotationRadians, 0.0f, 0.0f);

    UpdateRotation(appendage_2_circlePositions, appendage_2_initialCirclePositions, appendage_2_numCircleVertices, initialRotationRadians, 0.5f, 0.5f);
    UpdateRotation(appendage_2_rectanglePositions, appendage_2_initialRectanglePositions, appendage_2_numRectangleVertices, initialRotationRadians, 0.5f, 0.5f);

    shader.Bind();
}

void RobotArm::InitializeAppendage(unsigned int& circleVao, unsigned int& circleVbo, unsigned int& circleEbo,
    unsigned int& rectangleVao, unsigned int& rectangleVbo, unsigned int& rectangleEbo,
    float* circlePositions, float* rectanglePositions, unsigned int* circleIndices, unsigned int* rectangleIndices,
    int& numCircleVertices, int& numRectangleVertices, int& numCircleIndices, int& numRectangleIndices) {
    int index = 0;

    // Left half-circle vertices (from -?/2 to ?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = -M_PI / 2 + M_PI * i / 20;
        circlePositions[index++] = scale * (0.4f + 0.2f * cos(theta));
        circlePositions[index++] = scale * (0.2f * sin(theta));
    }

    // Right half-circle vertices (from ?/2 to 3?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = M_PI / 2 + M_PI * i / 20;
        circlePositions[index++] = scale * (-0.4f + 0.2f * cos(theta));
        circlePositions[index++] = scale * (0.2f * sin(theta));
    }

    // Rectangle vertices
    rectanglePositions[0] = scale * 0.4f;  // Top right
    rectanglePositions[1] = scale * 0.2f;
    rectanglePositions[2] = scale * 0.4f;  // Bottom right
    rectanglePositions[3] = scale * -0.2f;
    rectanglePositions[4] = scale * -0.4f; // Bottom left
    rectanglePositions[5] = scale * -0.2f;
    rectanglePositions[6] = scale * -0.4f; // Top left
    rectanglePositions[7] = scale * 0.2f;

    // Translate the entire geometry so that the center of the left half-circle is at the origin
    TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

    index = 0;

    // Left half-circle indices
    for (int i = 0; i < 20; ++i) {
        circleIndices[index++] = 0;
        circleIndices[index++] = i;
        circleIndices[index++] = i + 1;
    }

    // Right half-circle indices
    for (int i = 21; i < 41; ++i) {
        circleIndices[index++] = 21;
        circleIndices[index++] = i;
        circleIndices[index++] = i + 1;
    }

    // Rectangle indices
    rectangleIndices[0] = 0; // Top right
    rectangleIndices[1] = 1; // Bottom right
    rectangleIndices[2] = 2; // Bottom left
    rectangleIndices[3] = 0; // Top right
    rectangleIndices[4] = 2; // Bottom left
    rectangleIndices[5] = 3; // Top left

    // Vertex Array Object and Buffer for circles
    GLCall(glGenVertexArrays(1, &circleVao));
    GLCall(glGenBuffers(1, &circleVbo));
    GLCall(glGenBuffers(1, &circleEbo));

    // Initialize VAO and VBO for circles
    GLCall(glBindVertexArray(circleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numCircleVertices * 2, circlePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, circleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numCircleIndices, circleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Vertex Array Object and Buffer for rectangle
    GLCall(glGenVertexArrays(1, &rectangleVao));
    GLCall(glGenBuffers(1, &rectangleVbo));
    GLCall(glGenBuffers(1, &rectangleEbo));

    // Initialize VAO and VBO for rectangle
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numRectangleVertices * 2, rectanglePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rectangleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numRectangleIndices, rectangleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));
}

void RobotArm::Update() {
    if (newInputReceived) {
        // Update positions with rotation based on new input
        UpdateRotation(appendage_1_circlePositions, appendage_1_initialCirclePositions, appendage_1_numCircleVertices, rotationSpeed, 0.0f, 0.0f);
        UpdateRotation(appendage_1_rectanglePositions, appendage_1_initialRectanglePositions, appendage_1_numRectangleVertices, rotationSpeed, 0.0f, 0.0f);

        UpdateRotation(appendage_2_circlePositions, appendage_2_initialCirclePositions, appendage_2_numCircleVertices, rotationSpeed, 0.5f, 0.5f);
        UpdateRotation(appendage_2_rectanglePositions, appendage_2_initialRectanglePositions, appendage_2_numRectangleVertices, rotationSpeed, 0.5f, 0.5f);

        // Update VBOs
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_1_circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_1_numCircleVertices * 2, appendage_1_circlePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_1_rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_1_numRectangleVertices * 2, appendage_1_rectanglePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_2_circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_2_numCircleVertices * 2, appendage_2_circlePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, appendage_2_rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * appendage_2_numRectangleVertices * 2, appendage_2_rectanglePositions));

        newInputReceived = false; // Reset the flag
    }
}

void RobotArm::Render() {
    int location = shader.GetUniformLocation("u_Color");

    // Draw appendage 1
    RenderAppendage(appendage_1_circleVao, appendage_1_rectangleVao, appendage_1_numCircleIndices, appendage_1_numRectangleIndices);

    // Draw appendage 2
    RenderAppendage(appendage_2_circleVao, appendage_2_rectangleVao, appendage_2_numCircleIndices, appendage_2_numRectangleIndices);
}

void RobotArm::UpdateRotation(float* positions, const float* initialPositions, int numVertices, float angle, float centerX, float centerY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        float x = initialPositions[i] - centerX;
        float y = initialPositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        positions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        positions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }
}

void RobotArm::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

void RobotArm::RenderAppendage(unsigned int circleVao, unsigned int rectangleVao, int numCircleIndices, int numRectangleIndices) {
    int location = shader.GetUniformLocation("u_Color");

    // Draw the outline
    GLCall(glLineWidth(2.0f)); // Set the line width for the outline
    GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f)); // Set color to black

    // Draw the circles outline
    GLCall(glBindVertexArray(circleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle outline
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, numRectangleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the fill
    GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver

    // Draw the circles fill
    GLCall(glBindVertexArray(circleVao));
    GLCall(glDrawElements(GL_TRIANGLES, numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle fill
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glDrawElements(GL_TRIANGLES, numRectangleIndices, GL_UNSIGNED_INT, nullptr));
}






