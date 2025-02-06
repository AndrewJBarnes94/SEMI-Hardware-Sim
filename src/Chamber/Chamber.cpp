#include <GL/glew.h>
#include "Chamber.h"
#include "../Shader.h"
#include "../ErrorHandling.h" // Assuming GLCall is defined here
#include <cmath>

const float M_PI = 3.14159265358979323846f;

Chamber::Chamber(float scale) : scale(scale) {
    numVertices = 7; // 6 vertices for the hexagon + 1 for the center
    numIndices = 18; // 6 triangles (3 indices each)
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
    // Define vertices for a hexagon with rotation
    const float angleIncrement = 2.0f * M_PI / 6.0f; // 360° / 6 sides
    const float offset = M_PI / 6.0f; // Rotate by 30° (?/6) so sides are vertical
    positions[0] = 0.0f; // Center x
    positions[1] = 0.0f; // Center y

    for (int i = 0; i < 6; ++i) {
        float angle = i * angleIncrement + offset; // Add offset for rotation
        positions[(i + 1) * 2] = cos(angle) * scale; // x coordinate
        positions[(i + 1) * 2 + 1] = sin(angle) * scale; // y coordinate
    }

    // Define indices for 6 triangles forming the hexagon
    int index = 0;
    for (int i = 1; i <= 6; ++i) {
        indices[index++] = 0;          // Center vertex
        indices[index++] = i;          // Current vertex
        indices[index++] = (i % 6) + 1; // Next vertex (wrap around to 1 after 6)
    }

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
        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Metallic gray
    }

    GLCall(glBindVertexArray(vao));
    GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));
    GLCall(glBindVertexArray(0));
}
