#pragma once
#ifndef LOADPORT_H
#define LOADPORT_H

#include "../Shader.h"

class Loadport {
public:
	Loadport(float scale);
	~Loadport();
	void Initialize();
	void Render(const Shader& shader);

private:
	float scale;
	int numVertices;
	int numIndices;
	float initialPositions[8];
	float positions[8];
	unsigned int* indices;
	unsigned int vao, vbo, ebo;
};

#endif