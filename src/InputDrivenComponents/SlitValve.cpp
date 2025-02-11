#include <GL/glew.h>
#include "SlitValve.h"
#include "../ErrorHandling.h"
#include <cmath>

const float M_PI = 3.14159265358979323846f;

SlitValve::SlitValve(
    float scale,
    float topRightX,
    float topRightY,
    float bottomRightX,
    float bottomRightY,
    float bottomLeftX,
    float bottomLeftY,
    float topLeftX,
    float topLeftY
) : scale(scale),
topRightX(topRightX),
topRightY(topRightY),
bottomRightX(bottomRightX),
bottomRightY(bottomRightY),
bottomLeftX(bottomLeftX),
bottomLeftY(bottomLeftY),
topLeftX(topLeftX),
topLeftY(topLeftY) {

    numVertices = 4;  // 4 vertices for the square
    numIndices = 6;   // 2 triangles (3 indices each)

    // Allocate memory for indices
    indices = new unsigned int[numIndices];

    // Initialize indices
    indices[0] = 0; // Top right
    indices[1] = 1; // Bottom right
    indices[2] = 2; // Bottom left
    indices[3] = 0; // Top right
    indices[4] = 2; // Bottom left
    indices[5] = 3; // Top left
}

SlitValve::~SlitValve() {
    GLCall(glDeleteVertexArrays(1, &vao));
    GLCall(glDeleteBuffers(1, &vbo));
    GLCall(glDeleteBuffers(1, &ebo));
    delete[] indices; // Free dynamically allocated memory
}

void SlitValve::Initialize() {
    // Define positions
    positions[0] = scale * topRightX; // Top right
    initialPositions[0] = positions[0];
    positions[1] = scale * topRightY;
    initialPositions[1] = positions[1];

    positions[2] = scale * bottomRightX; // Bottom right
    initialPositions[2] = positions[2];
    positions[3] = scale * bottomRightY;
    initialPositions[3] = positions[3];

    positions[4] = scale * bottomLeftX; // Bottom left
    initialPositions[4] = positions[4];
    positions[5] = scale * bottomLeftY;
    initialPositions[5] = positions[5];

    positions[6] = scale * topLeftX; // Top left
    initialPositions[6] = positions[6];
    positions[7] = scale * topLeftY;
    initialPositions[7] = positions[7];

    // Generate buffers
    GLCall(glGenVertexArrays(1, &vao));
    GLCall(glGenBuffers(1, &vbo));
    GLCall(glGenBuffers(1, &ebo));

    GLCall(glBindVertexArray(vao));

    // Vertex buffer
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, vbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, numVertices * 2 * sizeof(float), positions, GL_DYNAMIC_DRAW));

    // Element buffer
    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, numIndices * sizeof(unsigned int), indices, GL_STATIC_DRAW));

    // Vertex attributes
    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Unbind to avoid accidental modification
    GLCall(glBindVertexArray(0));
}

void SlitValve::Render(const Shader& shader) {
    shader.Bind();

    int location = shader.GetUniformLocation("u_Color");
    if (location == -1) {
        std::cerr << "Uniform 'u_Color' not found in shader program." << std::endl;
        return;
    }

    GLCall(glLineWidth(5.0f));

    // Draw black outline
    GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f));
    GLCall(glBindVertexArray(vao));
    GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

    // Draw gray interior
    GLCall(glUniform4f(location, 0.5f, 0.5f, 0.5f, 1.0f));
    GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

    GLCall(glBindVertexArray(0)); // Unbind after drawing

    shader.Unbind();
}

