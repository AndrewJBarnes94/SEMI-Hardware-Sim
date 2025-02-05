// RobotVacuumSeal.cpp
#include "RobotVacuumSeal.h"
#include <GL/glew.h>
#include <cmath>
#include <iostream>

const float M_PI = 3.14159265358979323846f;

RobotVacuumSeal::RobotVacuumSeal(float scale) : scale(scale) {
    numVertices = 41; // 40 segments + center
    numIndices = 120; // 40 triangles * 3 indices

    positions = new float[numVertices * 2];
    indices = new unsigned int[numIndices];
}

RobotVacuumSeal::~RobotVacuumSeal() {
    glDeleteVertexArrays(1, &vao);
    glDeleteBuffers(1, &vbo);
    glDeleteBuffers(1, &ebo);

    delete[] positions;
    delete[] indices;
}

void RobotVacuumSeal::GenerateCircleVertices() {
    int index = 0;

    // Center vertex
    positions[index++] = 0.0f;
    positions[index++] = 0.0f;

    // Circle vertices
    float radius = 0.18f; // Hardcoded smaller radius
    for (int i = 0; i <= 40; ++i) {
        float theta = 2.0f * M_PI * i / 40; // 40 segments for smoother circle
        positions[index++] = radius * cos(theta);
        positions[index++] = radius * sin(theta);
    }

    // Circle indices
    index = 0;
    for (int i = 1; i <= 40; ++i) {
        indices[index++] = 0;
        indices[index++] = i;
        indices[index++] = i + 1;
    }
    indices[index - 1] = 1; // Close the circle
}

void RobotVacuumSeal::Initialize() {
    GenerateCircleVertices();

    // Generate VAO, VBO, and EBO
    glGenVertexArrays(1, &vao);
    glGenBuffers(1, &vbo);
    glGenBuffers(1, &ebo);

    glBindVertexArray(vao);

    glBindBuffer(GL_ARRAY_BUFFER, vbo);
    glBufferData(GL_ARRAY_BUFFER, numVertices * 2 * sizeof(float), positions, GL_STATIC_DRAW);

    glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo);
    glBufferData(GL_ELEMENT_ARRAY_BUFFER, numIndices * sizeof(unsigned int), indices, GL_STATIC_DRAW);

    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);

    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glBindVertexArray(0);
}

void RobotVacuumSeal::Render(const Shader& shader) {
    shader.Bind();

    int location = shader.GetUniformLocation("u_Color");
    if (location != -1) {
        // Metallic grey color
        glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f);
    }
    else {
        std::cerr << "Uniform 'u_Color' not found or optimized out!" << std::endl;
    }

    glBindVertexArray(vao);
    glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr);
    glBindVertexArray(0);

    shader.Unbind();
}
