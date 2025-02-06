#include <GL/glew.h>
#include "Chamber.h"
#include "../Shader.h"
#include "../ErrorHandling.h" // Assuming GLCall is defined here

Chamber::Chamber(float scale) : scale(scale) {
    numVertices = 4;
    numIndices = 6;
    positions = new float[numVertices * 2]; // Each vertex has x and y coordinates
    indices = new unsigned int[numIndices];
}

Chamber::~Chamber() {
    GLCall(glDeleteVertexArrays(1, &vao));
    GLCall(glDeleteBuffers(1, &vbo));
    GLCall(glDeleteBuffers(1, &ebo));
    delete[] positions;
    delete[] indices;
}

void Chamber::Initialize() {
    // Define vertices (x, y pairs for a square)
    positions[0] = -0.5f * scale; positions[1] = -0.5f * scale;
    positions[2] = 0.5f * scale; positions[3] = -0.5f * scale;
    positions[4] = 0.5f * scale; positions[5] = 0.5f * scale;
    positions[6] = -0.5f * scale; positions[7] = 0.5f * scale;

    // Define indices for two triangles forming a square
    indices[0] = 0; indices[1] = 1; indices[2] = 2;
    indices[3] = 2; indices[4] = 3; indices[5] = 0;

    GLCall(glGenVertexArrays(1, &vao));
    GLCall(glGenBuffers(1, &vbo));
    GLCall(glGenBuffers(1, &ebo));

    GLCall(glBindVertexArray(vao));

    // Vertex Buffer (VBO)
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, vbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, numVertices * 2 * sizeof(float), positions, GL_STATIC_DRAW));

    // Element Buffer (EBO)
    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, numIndices * sizeof(unsigned int), indices, GL_STATIC_DRAW));

    // Vertex Attribute Pointer (Position)
    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0));
    GLCall(glEnableVertexAttribArray(0));

    GLCall(glBindVertexArray(0)); // Unbind VAO
}

void Chamber::Render(const Shader& shader) {
    shader.Bind(); // Bind shader before setting uniform

    int location = shader.GetUniformLocation("u_Color");
    if (location != -1) {
        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f));
    }

    GLCall(glBindVertexArray(vao));
    GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));
    GLCall(glBindVertexArray(0));
}
