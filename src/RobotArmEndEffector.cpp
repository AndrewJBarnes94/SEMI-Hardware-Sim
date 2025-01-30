// File: src/RobotArmEndEffector.cpp

#include "RobotArmEndEffector.h"
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArmEndEffector::RobotArmEndEffector(float scale) : scale(scale) {
    numCircleVertices = (20 + 1);
    numRectangleVertices = 4;
    numCircleIndices = 3 * 20;
    numRectangleIndices = 6;
}

RobotArmEndEffector::~RobotArmEndEffector() {
    GLCall(glDeleteVertexArrays(1, &circleVao));
    GLCall(glDeleteBuffers(1, &circleVbo));
    GLCall(glDeleteBuffers(1, &circleEbo));
    GLCall(glDeleteVertexArrays(1, &rectangleVao));
    GLCall(glDeleteBuffers(1, &rectangleVbo));
    GLCall(glDeleteBuffers(1, &rectangleEbo));
}

void RobotArmEndEffector::Initialize() {
    int index = 0;

    // Left half-circle vertices (from -?/2 to ?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = -M_PI / 2 + M_PI * i / 20;
        circlePositions[index] = scale * (0.4f + 0.2f * cos(theta));
        initialCirclePositions[index] = circlePositions[index];
        index++;
        circlePositions[index] = scale * (0.2f * sin(theta));
        initialCirclePositions[index] = circlePositions[index];
        index++;
    }

    // Rectangle vertices
    rectanglePositions[0] = scale * 0.4f;  // Top right
    initialRectanglePositions[0] = rectanglePositions[0];
    rectanglePositions[1] = scale * 0.2f;
    initialRectanglePositions[1] = rectanglePositions[1];
    rectanglePositions[2] = scale * 0.4f;  // Bottom right
    initialRectanglePositions[2] = rectanglePositions[2];
    rectanglePositions[3] = scale * -0.2f;
    initialRectanglePositions[3] = rectanglePositions[3];
    rectanglePositions[4] = scale * -0.4f; // Bottom left
    initialRectanglePositions[4] = rectanglePositions[4];
    rectanglePositions[5] = scale * -0.2f;
    initialRectanglePositions[5] = rectanglePositions[5];
    rectanglePositions[6] = scale * -0.4f; // Top left
    initialRectanglePositions[6] = rectanglePositions[6];
    rectanglePositions[7] = scale * 0.2f;
    initialRectanglePositions[7] = rectanglePositions[7];

    // Translate the entire geometry so that the center of the left half-circle is at the origin
    TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

    TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);

    index = 0;

    // Left half-circle indices
    for (int i = 0; i < 20; ++i) {
        circleIndices[index++] = 0;
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

void RobotArmEndEffector::UpdateRotation(float angle, float centerX, float centerY) {
        for (int i = 0; i < numCircleVertices * 2; i += 2) {
            float x = initialCirclePositions[i] - centerX;
            float y = initialCirclePositions[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            circlePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
            circlePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
        }

        for (int i = 0; i < numRectangleVertices * 2; i += 2) {
            float x = initialRectanglePositions[i] - centerX;
            float y = initialRectanglePositions[i + 1] - centerY;

            // Apply rotation (negate angle for clockwise rotation)
            rectanglePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
            rectanglePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
        }

        // Update the vertex buffer with the new positions
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numCircleVertices * 2, circlePositions));

        GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRectangleVertices * 2, rectanglePositions));
    }

void RobotArmEndEffector::TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
    // Update the vertex buffer with the new positions
    if (positions == circlePositions) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numCircleVertices * 2, circlePositions));
    }
    else if (positions == rectanglePositions) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRectangleVertices * 2, rectanglePositions));
    }
}

void RobotArmEndEffector::Render(const Shader& shader) {
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

std::pair<float, float> RobotArmEndEffector::CalculateRectangleHeightMidpoint() const {
    // Midpoint of the rectangle's height on the left side
    float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
    float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
    return { midpointX, midpointY };
}

void RobotArmEndEffector::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {

}

std::pair<float, float> RobotArmEndEffector::CalculateRedDotPosition() const {
    // Calculate the red dot position based on the rectangle's height midpoint
    return CalculateRectangleHeightMidpoint();
}

void RobotArmEndEffector::TranslateToPosition(float x, float y) {
    // Calculate the offset needed to translate the appendage to the specified position
    auto redDotPosition = CalculateRedDotPosition();
    float offsetX = x - redDotPosition.first;
    float offsetY = y - redDotPosition.second;

    // Translate the entire geometry
    TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
    TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
}
