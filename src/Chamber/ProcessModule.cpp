#include <GL/glew.h>
#include "ProcessModule.h"
#include "../ErrorHandling.h"
#include <cmath>

const float M_PI = 3.14159265358979323846f;

ProcessModule::ProcessModule(float scale) : scale(scale) {
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

ProcessModule::~ProcessModule() {
    GLCall(glDeleteVertexArrays(1, &vao));
    GLCall(glDeleteBuffers(1, &vbo));
    GLCall(glDeleteBuffers(1, &ebo));
    delete[] indices; // Free dynamically allocated memory
}

void ProcessModule::Initialize() {
    // Define positions
    positions[0] = scale * -0.4f; // Top right
    initialPositions[0] = positions[0];
    positions[1] = scale * 0.2f;
    initialPositions[1] = positions[1];

    positions[2] = scale * -0.4f; // Bottom right
    initialPositions[2] = positions[2];
    positions[3] = scale * -0.2f;
    initialPositions[3] = positions[3];

    positions[4] = scale * -0.6f; // Bottom left
    initialPositions[4] = positions[4];
    positions[5] = scale * -0.2f;
    initialPositions[5] = positions[5];

    positions[6] = scale * -0.6f; // Top left
    initialPositions[6] = positions[6];
    positions[7] = scale * 0.2f;
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

void ProcessModule::Render(const Shader& shader) {
    shader.Bind();

    int location = shader.GetUniformLocation("u_Color");
    if (location != -1) { // Fix: -1 means uniform not found
        GLCall(glLineWidth(5.0f));

        // Draw black outline
        GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f));
        GLCall(glBindVertexArray(vao));
        GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

        // Draw gray interior
        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f));
        GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

        GLCall(glBindVertexArray(0)); // Unbind after drawing
    }

    shader.Unbind();
}
