#include <GL/glew.h>
#include "Chamber.h"
#include "../Shader.h"
#include "../ErrorHandling.h" // Assuming GLCall is defined here
#include <cmath>
#include <iostream>

#define _USE_MATH_DEFINES // Ensure M_PI is defined
#include <math.h>

Chamber::Chamber(float scale) : scale(scale) {
    numVertices = 7; // 6 vertices for the hexagon + 1 for the center
    numIndices = 18; // 6 triangles (3 indices each)

    numExtensionVertices1 = 4;
    numExtensionIndices1 = 6;

    numExtensionVertices2 = 4;
    numExtensionIndices2 = 6;

    positions = new float[numVertices * 2]; // Each vertex has x and y coordinates
    indices = new unsigned int[numIndices];

    extensionIndices1 = new unsigned int[numExtensionIndices1];
    extensionPositions1 = new float[numExtensionVertices1 * 2];

    extensionIndices2 = new unsigned int[numExtensionIndices2];
    extensionPositions2 = new float[numExtensionVertices2 * 2];
}

Chamber::~Chamber() {
    GLCall(glDeleteVertexArrays(1, &vao));
    GLCall(glDeleteBuffers(1, &vbo));
    GLCall(glDeleteBuffers(1, &ebo));

    GLCall(glDeleteVertexArrays(1, &extensionVao1));
    GLCall(glDeleteBuffers(1, &extensionVbo1));
    GLCall(glDeleteBuffers(1, &extensionEbo1));

    GLCall(glDeleteVertexArrays(1, &extensionVao2));
    GLCall(glDeleteBuffers(1, &extensionVbo2));
    GLCall(glDeleteBuffers(1, &extensionEbo2));

    delete[] positions;
    delete[] indices;

    delete[] extensionIndices1;
    delete[] extensionPositions1;

    delete[] extensionIndices2;
    delete[] extensionIndices2;
}

void Chamber::Initialize() {
    if (glewInit() != GLEW_OK) {
        std::cerr << "Error initializing GLEW" << std::endl;
        return;
    }

    // Define vertices for a hexagon with rotation
    const float angleIncrement = 2.0f * static_cast<float>(M_PI) / 6.0f; // 360° / 6 sides
    const float offset = static_cast<float>(M_PI) / 6.0f; // Rotate by 30° so sides are vertical
    positions[0] = 0.0f; // Center x
    positions[1] = 0.0f; // Center y

    for (int i = 0; i < 6; ++i) {
        float angle = i * angleIncrement + offset;
        positions[(i + 1) * 2] = cos(angle) * scale;
        positions[(i + 1) * 2 + 1] = sin(angle) * scale;
    }

    // Define indices for 6 triangles forming the hexagon
    int index = 0;
    for (int i = 1; i <= 6; ++i) {
        indices[index++] = 0; // Center vertex
        indices[index++] = i;
        indices[index++] = (i % 6) + 1;
    }

    extensionPositions1[0] = -0.205681f;
    extensionPositions1[1] = -0.08875f;
    extensionPositions1[2] = 0.0f;
    extensionPositions1[3] = -0.2075f;
    extensionPositions1[4] = 0.0f;
    extensionPositions1[5] = -0.28f;
    extensionPositions1[6] = -0.205681f;
    extensionPositions1[7] = -0.19f;

    extensionIndices1[0] = 0;
    extensionIndices1[1] = 1;
    extensionIndices1[2] = 2;
    extensionIndices1[3] = 0;
    extensionIndices1[4] = 2;
    extensionIndices1[5] = 3;

    extensionPositions2[0] = 0.205681f;
    extensionPositions2[1] = -0.08875f;
    extensionPositions2[2] = 0.0f;
    extensionPositions2[3] = -0.2075f;
    extensionPositions2[4] = 0.0f;
    extensionPositions2[5] = -0.28f;
    extensionPositions2[6] = 0.205681f;
    extensionPositions2[7] = -0.19f;

    extensionIndices2[0] = 0;
    extensionIndices2[1] = 1;
    extensionIndices2[2] = 2;
    extensionIndices2[3] = 0;
    extensionIndices2[4] = 2;
    extensionIndices2[5] = 3;

    // Ensure buffers are initialized correctly
    GLCall(glGenVertexArrays(1, &vao));
    GLCall(glGenBuffers(1, &vbo));
    GLCall(glGenBuffers(1, &ebo));

    if (vao == 0 || vbo == 0 || ebo == 0) {
        std::cerr << "Error: VAO, VBO, or EBO not initialized correctly" << std::endl;
        return;
    }

    // Generate and bind VAO for the hexagon
    GLCall(glGenVertexArrays(1, &vao));
    GLCall(glBindVertexArray(vao));

    GLCall(glGenBuffers(1, &vbo));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, vbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, numVertices * 2 * sizeof(float), positions, GL_STATIC_DRAW));

    GLCall(glGenBuffers(1, &ebo));
    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ebo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, numIndices * sizeof(unsigned int), indices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0));
    GLCall(glEnableVertexAttribArray(0));

    GLCall(glBindVertexArray(0)); // Unbind VAO

    // Initialize extension1 VAO
    GLCall(glGenVertexArrays(1, &extensionVao1));
    GLCall(glGenBuffers(1, &extensionVbo1));
    GLCall(glGenBuffers(1, &extensionEbo1));

    GLCall(glBindVertexArray(extensionVao1));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, extensionVbo1));
    GLCall(glBufferData(GL_ARRAY_BUFFER, numExtensionVertices1 * 2 * sizeof(float), extensionPositions1, GL_STATIC_DRAW));
    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, extensionEbo1));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, numExtensionIndices1 * sizeof(unsigned int), extensionIndices1, GL_STATIC_DRAW));
    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0));
    GLCall(glEnableVertexAttribArray(0));
    GLCall(glBindVertexArray(0));

    // Initialize extension2 VAO
    GLCall(glGenVertexArrays(1, &extensionVao2));
    GLCall(glGenBuffers(1, &extensionVbo2));
    GLCall(glGenBuffers(1, &extensionEbo2));

    GLCall(glBindVertexArray(extensionVao2));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, extensionVbo2));
    GLCall(glBufferData(GL_ARRAY_BUFFER, numExtensionVertices2 * 2 * sizeof(float), extensionPositions2, GL_STATIC_DRAW));
    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, extensionEbo2));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, numExtensionIndices2 * sizeof(unsigned int), extensionIndices2, GL_STATIC_DRAW));
    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0));
    GLCall(glEnableVertexAttribArray(0));
    GLCall(glBindVertexArray(0));

}

void Chamber::Render(const Shader& shader) {
    shader.Bind();
    int location = shader.GetUniformLocation("u_Color");

    if (location != -1) {
        GLCall(glLineWidth(5.0f));
        GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f));
        GLCall(glBindVertexArray(vao));
        GLCall(glDrawElements(GL_LINE_LOOP, 6, GL_UNSIGNED_INT, nullptr));

        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f));
        GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

        // Draw extension1
        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f));
        GLCall(glBindVertexArray(extensionVao1));
        GLCall(glDrawElements(GL_TRIANGLES, numExtensionIndices1, GL_UNSIGNED_INT, nullptr));

        // Draw extension2
        GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f));
        GLCall(glBindVertexArray(extensionVao2));
        GLCall(glDrawElements(GL_TRIANGLES, numExtensionIndices2, GL_UNSIGNED_INT, nullptr));

        std::map<std::string, std::vector<float>> positionMap = getPositionMap("none");
        if (!positionMap.empty()) {
            std::vector<float> position = positionMap.begin()->second;

            unsigned int pointVao, pointVbo;
            GLCall(glGenVertexArrays(1, &pointVao));
            GLCall(glGenBuffers(1, &pointVbo));

            GLCall(glBindVertexArray(pointVao));
            GLCall(glBindBuffer(GL_ARRAY_BUFFER, pointVbo));
            GLCall(glBufferData(GL_ARRAY_BUFFER, position.size() * sizeof(float), position.data(), GL_STATIC_DRAW));
            GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), (void*)0));
            GLCall(glEnableVertexAttribArray(0));

            GLCall(glPointSize(15.0f)); // Set the point size to make it larger
            GLCall(glUniform4f(location, 1.0f, 0.0f, 0.0f, 1.0f)); // Set color to red
            GLCall(glDrawArrays(GL_POINTS, 0, 1));

            GLCall(glBindVertexArray(0));
            GLCall(glDeleteVertexArrays(1, &pointVao));
            GLCall(glDeleteBuffers(1, &pointVbo));
        }

        GLenum err;
        while ((err = glGetError()) != GL_NO_ERROR) {
            std::cerr << "OpenGL Error: " << err << std::endl;
        }

        glBindVertexArray(0);
        shader.Unbind();
    }
}



std::vector<float> Chamber::getPositions() {
    std::vector<float> allPositions;

    allPositions.insert(allPositions.end(), positions, positions + numVertices * 2);
    allPositions.insert(allPositions.end(), extensionPositions1, extensionPositions1 + numExtensionVertices1 * 2);
    allPositions.insert(allPositions.end(), extensionPositions2, extensionPositions2 + numExtensionVertices2 * 2);

    return allPositions;
}

std::map<std::string, std::vector<float>> Chamber::getPositionMap(const std::string& point) {
    std::vector<float> positions = getPositions();

	std::map<std::string, std::vector<float>> positionMap;
    if (point == "center") {
        positionMap["center"] = { positions[0], positions[1] };
    }
    else if (point == "topRight") {
        positionMap["topRight"] = { positions[2], positions[3] };
    }
    else if (point == "top") {
        positionMap["top"] = { positions[4], positions[5] };
    }
    else if (point == "topLeft") {
        positionMap["topLeft"] = { positions[6], positions[7] };
    }
    else if (point == "bottomLeft") {
        positionMap["bottomLeft"] = { positions[20], positions[21] };
    }
    else if (point == "bottom") {
        positionMap["bottom"] = { positions[26], positions[27] };
    }
    else if (point == "bottomRight") {
        positionMap["bottomRight"] = { positions[28], positions[29] };
    }
    else {
        // do nothing
    }
	return positionMap;
}