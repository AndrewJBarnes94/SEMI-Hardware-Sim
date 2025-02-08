#pragma once
#ifndef PROCESSMODULE_H
#define PROCESSMODULE_H

#include "../Shader.h"

class ProcessModule {
public:
    ProcessModule(
        float scale,
        float topRightX,
        float topRightY,
        float bottomRightX,
        float bottomRightY,
        float bottomLeftX,
        float bottomLeftY,
        float topLeftX,
        float topLeftY
    );
    ~ProcessModule();
    void Initialize();
    void Render(const Shader& shader);

private:
    float scale;
    int numVertices;
    int numIndices;
    float initialPositions[8];
    float positions[8];
    unsigned int* indices; // Now properly allocated in constructor
    unsigned int vao, vbo, ebo;

	float topRightX, topRightY;
	float bottomRightX, bottomRightY;
	float bottomLeftX, bottomLeftY;
	float topLeftX, topLeftY;
};

#endif // PROCESSMODULE_H
