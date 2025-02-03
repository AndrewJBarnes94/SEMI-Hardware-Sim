#pragma once

#include "Shader.h"
#include <utility>

class RobotArmEndEffector {
public:
    RobotArmEndEffector(float scale);
    ~RobotArmEndEffector();

    void Initialize();
    void UpdateRotation(float angle, float centerX, float centerY);
    void TranslateToPosition(float x, float y);
    void Render(const Shader& shader);
    std::pair<float, float> CalculateRedDotPosition() const;

private:
    void TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY);
    std::pair<float, float> CalculateRectangleHeightMidpoint() const;
    void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);

    float scale;
    int numCircleVertices;
    int numRectangleVertices;
    int numSquareVertices;
    int numRightTriangleVertices1;
	int numRightTriangleVertices2;
    int numSmallRectangleVertices1;
	int numSmallRectangleVertices2;

    int numCircleIndices;
    int numRectangleIndices;
	int numSquareIndices;
	int numRightTriangleIndices1;
	int numRightTriangleIndices2;
	int numSmallRectangleIndices1;
	int numSmallRectangleIndices2;
    
    float circlePositions[42]; // 21 vertices * 2 (x, y)
    float initialCirclePositions[42];
    unsigned int circleIndices[60]; // 20 triangles * 3 indices
    
    float rectanglePositions[8]; // 4 vertices * 2 (x, y)
    float initialRectanglePositions[8];
    unsigned int rectangleIndices[6]; // 2 triangles * 3 indices

	float squarePositions[8]; // 4 vertices * 2 (x, y)
	float initialSquarePositions[8];
	unsigned int squareIndices[6]; // 2 triangles * 3 indices

	float rightTrianglePositions1[6]; // 3 vertices * 2 (x, y)
	float initialRightTrianglePositions1[6];
	unsigned int rightTriangleIndices1[3]; // 1 triangle * 3 indices

	float rightTrianglePositions2[6]; // 3 vertices * 2 (x, y)
	float initialRightTrianglePositions2[6];
	unsigned int rightTriangleIndices2[3]; // 1 triangle * 3 indices

	float smallRectanglePositions1[8]; // 4 vertices * 2 (x, y)
	float initialSmallRectanglePositions1[8];
	unsigned int smallRectangleIndices1[6]; // 2 triangles * 3 indices

	float smallRectanglePositions2[8]; // 4 vertices * 2 (x, y)
	float initialSmallRectanglePositions2[8];
	unsigned int smallRectangleIndices2[6]; // 2 triangles * 3 indices

    
    unsigned int circleVao, circleVbo, circleEbo;
    unsigned int rectangleVao, rectangleVbo, rectangleEbo;
	unsigned int squareVao, squareVbo, squareEbo;
	unsigned int rightTriangleVao1, rightTriangleVbo1, rightTriangleEbo1;
	unsigned int rightTriangleVao2, rightTriangleVbo2, rightTriangleEbo2;
	unsigned int smallRectangleVao1, smallRectangleVbo1, smallRectangleEbo1;
	unsigned int smallRectangleVao2, smallRectangleVbo2, smallRectangleEbo2;
};

