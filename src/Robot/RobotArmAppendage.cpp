// File: src/RobotArmAppendage.cpp
#include "RobotArmAppendage.h"
#include <cmath>
#include "../ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArmAppendage::RobotArmAppendage(float scale) : scale(scale) {
    numCircleVertices = 2 * (20 + 1);
    numRectangleVertices = 4;
    numCircleIndices = 2 * 3 * 20; // Initialize here
    numRectangleIndices = 6; // Initialize here
}

RobotArmAppendage::~RobotArmAppendage() {
    GLCall(glDeleteVertexArrays(1, &circleVao));
    GLCall(glDeleteBuffers(1, &circleVbo));
    GLCall(glDeleteBuffers(1, &circleEbo));
    GLCall(glDeleteVertexArrays(1, &rectangleVao));
    GLCall(glDeleteBuffers(1, &rectangleVbo));
    GLCall(glDeleteBuffers(1, &rectangleEbo));
}

void RobotArmAppendage::Initialize() {
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

    // Right half-circle vertices (from ?/2 to 3?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = M_PI / 2 + M_PI * i / 20;
        circlePositions[index] = scale * (-0.4f + 0.2f * cos(theta));
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

void RobotArmAppendage::UpdateRotation(float angle, float centerX, float centerY) {
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

void RobotArmAppendage::TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY) {
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

void RobotArmAppendage::Render(const Shader& shader) {
    shader.Bind(); // Ensure the shader is bound

    int location = shader.GetUniformLocation("u_Color");

    if (location != -1) { // Check if the uniform location is valid
        // Draw the outline
        GLCall(glLineWidth(5.0f)); // Set the line width for the outline
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
    else {
        std::cerr << "Uniform location for 'u_Color' is invalid." << std::endl;
    }

    glBindVertexArray(0); // Unbind the VAO
    shader.Unbind(); // Unbind the shader program
}


std::pair<float, float> RobotArmAppendage::CalculateRectangleHeightMidpoint(std::string side) const {
    // Midpoint of the rectangle's height on the left side
    if (side == "left") {
        float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
        float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
        return { midpointX, midpointY };
    }
    // Midpoint of the rectangle's height on the right side
    else if (side == "right") {
        float midpointX = (rectanglePositions[4] + rectanglePositions[6]) / 2;
        float midpointY = (rectanglePositions[5] + rectanglePositions[7]) / 2;
        return { midpointX, midpointY };
    }
    // Default return value for unexpected side values
    return { 0.0f, 0.0f };
}


void RobotArmAppendage::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

std::pair<float, float> RobotArmAppendage::CalculateRedDotPosition(std::string side) const {
    // Calculate the red dot position based on the rectangle's height midpoint
    return CalculateRectangleHeightMidpoint(side);
}

void RobotArmAppendage::TranslateToPosition(float x, float y) {
    // Calculate the offset needed to translate the appendage to the specified position
    auto redDotPosition = CalculateRedDotPosition("right");
    float offsetX = x - redDotPosition.first;
    float offsetY = y - redDotPosition.second;

    // Translate the entire geometry
    TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
    TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
}