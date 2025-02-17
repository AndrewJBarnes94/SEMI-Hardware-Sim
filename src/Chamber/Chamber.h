#pragma once
#ifndef CHAMBER_H
#define CHAMBER_H

#include "../Shader.h"
#include <vector>
#include <map>
#include <string>

class Chamber {
public:
    Chamber(float scale);
    ~Chamber();
    void Initialize();
    void Render(const Shader& shader);

    std::vector<float> getPositionMap(const std::string& point);

private:
    float scale;

    int numVertices;
    int numIndices;
    float* positions;
    unsigned int* indices;
    unsigned int vao, vbo, ebo;

    int numExtensionVertices1;
    int numExtensionIndices1;
    float* extensionPositions1;
    unsigned int* extensionIndices1;
    unsigned int extensionVao1, extensionVbo1, extensionEbo1;

    int numExtensionVertices2;
    int numExtensionIndices2;
    float* extensionPositions2;
    unsigned int* extensionIndices2;
    unsigned int extensionVao2, extensionVbo2, extensionEbo2;
    
    std::vector<float> getPositions();
};

#endif // CHAMBER_H

