#pragma once
#ifndef CHAMBER_H
#define CHAMBER_H

#include "../Shader.h"

class Chamber {
public:
	Chamber(float scale);
	~Chamber();
	void Initialize();
	void Render(const Shader& shader);

private:
	float scale;
	int numVertices;
	int numIndices;
	float* positions;
	unsigned int* indices;
	unsigned int vao, vbo, ebo;
};

#endif // CHAMBER_H