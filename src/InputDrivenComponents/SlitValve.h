#pragma once
#ifndef SLITVALVE_H
#define SLITVALVE_H

#include "../Shader.h"

class SlitValve {
public:
    SlitValve(
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
    ~SlitValve();
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

    float topRightX, topRightY;
    float bottomRightX, bottomRightY;
    float bottomLeftX, bottomLeftY;
    float topLeftX, topLeftY;
};

#endif