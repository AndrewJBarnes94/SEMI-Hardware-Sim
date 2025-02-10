#include <GL/glew.h>
#include "Loadport.h"
#include "../ErrorHandling.h"
#include <cmath>

const float M_PI = 3.14159265358979323846f;

Loadport::Loadport(float scale) : scale(scale) {
	numVertices = 4;
	numIndices = 6;

	indices = new unsigned int[numIndices];

	indices[0] = 0;
	indices[1] = 1;
	indices[2] = 2;
	indices[3] = 0;
	indices[4] = 2;
	indices[5] = 3;
}

Loadport::~Loadport() {
	GLCall(glDeleteVertexArrays(1, &vao));
	GLCall(glDeleteBuffers(1, &vbo));
	GLCall(glDeleteBuffers(1, &ebo));

	delete[] indices;
}

void Loadport::Initialize() {
	positions[0] = scale * 0.4f;
	initialPositions[0] = positions[0];
	positions[1] = scale * -0.8f;
	initialPositions[1] = positions[1];

	positions[2] = scale * 0.4f;
	initialPositions[2] = positions[2];
	positions[3] = scale * -0.9f;
	initialPositions[3] = positions[3];

	positions[4] = scale * -0.4f;
	initialPositions[4] = positions[4];
	positions[5] = scale * -0.9f;
	initialPositions[5] = positions[5];

	positions[6] = scale * -0.4f;
	initialPositions[6] = positions[6];
	positions[7] = scale * -0.8f;
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

void Loadport::Render(const Shader& shader) {
	shader.Bind();

	int location = shader.GetUniformLocation("u_Color");
	if (location != -1) {
		GLCall(glLineWidth(5.0f));

		// Draw black outline
		GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f));
		GLCall(glBindVertexArray(vao));
		GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

		// Fill geometry
		GLCall(glUniform4f(location, 0.2f, 0.2f, 0.2f, 1.0f));
		GLCall(glDrawElements(GL_TRIANGLES, numIndices, GL_UNSIGNED_INT, nullptr));

		GLCall(glBindVertexArray(0));
	}
	
	shader.Unbind();
}