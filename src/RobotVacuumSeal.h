#pragma once

#include "Shader.h"
#include <utility>

class RobotVacuumSeal {
public:
    RobotVacuumSeal(float scale);
    ~RobotVacuumSeal();

    void Initialize();
    void Render(const Shader& shader);
    void SetRadius(float radius);

private:
    float scale;
    float radius;
    int numVertices;
    int numIndices;
    float* positions;
    unsigned int* indices;
    unsigned int vao, vbo, ebo;

    void GenerateCircleVertices();
};
