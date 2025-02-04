#include "RobotArmEndEffector.h"
#include <cmath>
#include "ErrorHandling.h"

const float M_PI = 3.14159265358979323846f;

RobotArmEndEffector::RobotArmEndEffector(float scale) : scale(scale) {
    numCircleVertices = (20 + 1);
    numRectangleVertices = 4;
    numSquareVertices = 4;
	numRightTriangleVertices1 = 3;
    numRightTriangleVertices2 = 3;
    numRightTriangleVertices3 = 3;
	numRightTriangleVertices4 = 3;
	numSmallRectangleVertices1 = 4;
    numSmallRectangleVertices2 = 4;

    numCircleIndices = 3 * 20;
    numRectangleIndices = 6;
    numSquareIndices = 6;   
	numRightTriangleIndices1 = 3;
    numRightTriangleIndices2 = 3;
	numRightTriangleIndices3 = 3;
	numRightTriangleIndices4 = 3;
	numSmallRectangleIndices1 = 6;
    numSmallRectangleIndices2 = 6;
}

RobotArmEndEffector::~RobotArmEndEffector() {
    GLCall(glDeleteVertexArrays(1, &circleVao));
    GLCall(glDeleteBuffers(1, &circleVbo));
    GLCall(glDeleteBuffers(1, &circleEbo));

    GLCall(glDeleteVertexArrays(1, &rectangleVao));
    GLCall(glDeleteBuffers(1, &rectangleVbo));
    GLCall(glDeleteBuffers(1, &rectangleEbo));

    GLCall(glDeleteVertexArrays(1, &squareVao));
    GLCall(glDeleteBuffers(1, &squareVbo));
    GLCall(glDeleteBuffers(1, &squareEbo));

	GLCall(glDeleteBuffers(1, &rightTriangleVbo1));
	GLCall(glDeleteBuffers(1, &rightTriangleEbo1));
	GLCall(glDeleteVertexArrays(1, &rightTriangleVao1));

    GLCall(glDeleteBuffers(1, &rightTriangleVbo2));
    GLCall(glDeleteBuffers(1, &rightTriangleEbo2));
    GLCall(glDeleteVertexArrays(1, &rightTriangleVao2));

	GLCall(glDeleteBuffers(1, &rightTriangleVbo3));
	GLCall(glDeleteBuffers(1, &rightTriangleEbo3));
	GLCall(glDeleteVertexArrays(1, &rightTriangleVao3));

	GLCall(glDeleteBuffers(1, &rightTriangleVbo4));
	GLCall(glDeleteBuffers(1, &rightTriangleEbo4));
	GLCall(glDeleteVertexArrays(1, &rightTriangleVao4));

	GLCall(glDeleteBuffers(1, &smallRectangleVbo1));
	GLCall(glDeleteBuffers(1, &smallRectangleEbo1));
	GLCall(glDeleteVertexArrays(1, &smallRectangleVao1));

	GLCall(glDeleteBuffers(1, &smallRectangleVbo2));
	GLCall(glDeleteBuffers(1, &smallRectangleEbo2));
	GLCall(glDeleteVertexArrays(1, &smallRectangleVao2));
}

void RobotArmEndEffector::Initialize() {
    int index = 0;

    // Left half-circle vertices (from -?/2 to ?/2)
    for (int i = 0; i <= 20; ++i) {
        float theta = -M_PI / 2 + M_PI * i / 20;
        circlePositions[index] = scale * (0.4f + 0.2f * cos(theta));
        initialCirclePositions[index] = circlePositions[index];
        index++;
        circlePositions[index] = scale * (0.2f * sin(theta));
        initialCirclePositions[index] = circlePositions[index];
        index++;
    }

    // Rectangle vertices
    rectanglePositions[0] = scale * 0.4f;  // Top right
    initialRectanglePositions[0] = rectanglePositions[0];
    rectanglePositions[1] = scale * 0.2f;
    initialRectanglePositions[1] = rectanglePositions[1];
    rectanglePositions[2] = scale * 0.4f;  // Bottom right
    initialRectanglePositions[2] = rectanglePositions[2];
    rectanglePositions[3] = scale * -0.2f;
    initialRectanglePositions[3] = rectanglePositions[3];
    rectanglePositions[4] = scale * -0.4f; // Bottom left
    initialRectanglePositions[4] = rectanglePositions[4];
    rectanglePositions[5] = scale * -0.2f;
    initialRectanglePositions[5] = rectanglePositions[5];
    rectanglePositions[6] = scale * -0.4f; // Top left
    initialRectanglePositions[6] = rectanglePositions[6];
    rectanglePositions[7] = scale * 0.2f;
    initialRectanglePositions[7] = rectanglePositions[7];

    // Square vertices
    squarePositions[0] = scale * -0.4f;  // Top right
    initialSquarePositions[0] = squarePositions[0];
    squarePositions[1] = scale * 0.2f;
    initialSquarePositions[1] = squarePositions[1];
    squarePositions[2] = scale * -0.4f; // Bottom right
    initialSquarePositions[2] = squarePositions[2];
    squarePositions[3] = scale * -0.2f;
    initialSquarePositions[3] = squarePositions[3];
    squarePositions[4] = scale * -0.6f; // Bottom left
    initialSquarePositions[4] = squarePositions[4];
    squarePositions[5] = scale * -0.2f;
    initialSquarePositions[5] = squarePositions[5];
    squarePositions[6] = scale * -0.6f;  // Top left
    initialSquarePositions[6] = squarePositions[6];
    squarePositions[7] = scale * 0.2f;
    initialSquarePositions[7] = squarePositions[7];

    // Right triangle1 vertices
    rightTrianglePositions1[0] = scale * -0.4f;  // Bottom right
    rightTrianglePositions1[1] = scale * 0.2f;
    rightTrianglePositions1[2] = scale * -0.6f;  // Bottom left
    rightTrianglePositions1[3] = scale * 0.2f;
    rightTrianglePositions1[4] = scale * -0.6f;  // Top left
    rightTrianglePositions1[5] = scale * 0.3f;

    initialRightTrianglePositions1[0] = rightTrianglePositions1[0];
    initialRightTrianglePositions1[1] = rightTrianglePositions1[1];
    initialRightTrianglePositions1[2] = rightTrianglePositions1[2];
    initialRightTrianglePositions1[3] = rightTrianglePositions1[3];
    initialRightTrianglePositions1[4] = rightTrianglePositions1[4];
    initialRightTrianglePositions1[5] = rightTrianglePositions1[5];

	// Right triangle2 vertices
	rightTrianglePositions2[0] = scale * -0.4f;  // Top right
	rightTrianglePositions2[1] = scale * -0.2f;
	rightTrianglePositions2[2] = scale * -0.6f;  // Top left
	rightTrianglePositions2[3] = scale * -0.2f;
	rightTrianglePositions2[4] = scale * -0.6f;  // Bottom left
	rightTrianglePositions2[5] = scale * -0.3f;

	initialRightTrianglePositions2[0] = rightTrianglePositions2[0];
	initialRightTrianglePositions2[1] = rightTrianglePositions2[1];
	initialRightTrianglePositions2[2] = rightTrianglePositions2[2];
	initialRightTrianglePositions2[3] = rightTrianglePositions2[3];
	initialRightTrianglePositions2[4] = rightTrianglePositions2[4];
	initialRightTrianglePositions2[5] = rightTrianglePositions2[5];

	// Right triangle3 vertices
	rightTrianglePositions3[0] = scale * -0.6f;  // Top right
	rightTrianglePositions3[1] = scale * 0.2f;
	rightTrianglePositions3[2] = scale * -0.6f;  // Bottom right
	rightTrianglePositions3[3] = scale * 0.1f;
	rightTrianglePositions3[4] = scale * -0.8f;  // Bottom left
	rightTrianglePositions3[5] = scale * 0.2f;

	initialRightTrianglePositions3[0] = rightTrianglePositions3[0];
	initialRightTrianglePositions3[1] = rightTrianglePositions3[1];
	initialRightTrianglePositions3[2] = rightTrianglePositions3[2];
	initialRightTrianglePositions3[3] = rightTrianglePositions3[3];
	initialRightTrianglePositions3[4] = rightTrianglePositions3[4];
	initialRightTrianglePositions3[5] = rightTrianglePositions3[5];

	// Right triangle4 vertices
	rightTrianglePositions4[0] = scale * -0.6f;  // Bottom right
	rightTrianglePositions4[1] = scale * -0.2f;
	rightTrianglePositions4[2] = scale * -0.6f;  // Bottom left
	rightTrianglePositions4[3] = scale * -0.1f;
	rightTrianglePositions4[4] = scale * -0.8f;  // Top left
	rightTrianglePositions4[5] = scale * -0.2f;

	initialRightTrianglePositions4[0] = rightTrianglePositions4[0];
	initialRightTrianglePositions4[1] = rightTrianglePositions4[1];
	initialRightTrianglePositions4[2] = rightTrianglePositions4[2];
	initialRightTrianglePositions4[3] = rightTrianglePositions4[3];
	initialRightTrianglePositions4[4] = rightTrianglePositions4[4];
	initialRightTrianglePositions4[5] = rightTrianglePositions4[5];

	// Small rectangle1 vertices
	smallRectanglePositions1[0] = scale * -0.6f;  // Top right
	smallRectanglePositions1[1] = scale * 0.3f;
	smallRectanglePositions1[2] = scale * -0.6f;  // Bottom right
	smallRectanglePositions1[3] = scale * 0.2f;
	smallRectanglePositions1[4] = scale * -1.0f;  // Bottom left
	smallRectanglePositions1[5] = scale * 0.2f;
	smallRectanglePositions1[6] = scale * -1.0f;  // Top left
	smallRectanglePositions1[7] = scale * 0.3f;

	initialSmallRectanglePositions1[0] = smallRectanglePositions1[0];
	initialSmallRectanglePositions1[1] = smallRectanglePositions1[1];
	initialSmallRectanglePositions1[2] = smallRectanglePositions1[2];
	initialSmallRectanglePositions1[3] = smallRectanglePositions1[3];
	initialSmallRectanglePositions1[4] = smallRectanglePositions1[4];
	initialSmallRectanglePositions1[5] = smallRectanglePositions1[5];
	initialSmallRectanglePositions1[6] = smallRectanglePositions1[6];
	initialSmallRectanglePositions1[7] = smallRectanglePositions1[7];

	// Small rectangle2 vertices
	smallRectanglePositions2[0] = scale * -0.6f;  // Top right
	smallRectanglePositions2[1] = scale * -0.2f;
	smallRectanglePositions2[2] = scale * -0.6f;  // Bottom right
	smallRectanglePositions2[3] = scale * -0.3f;
	smallRectanglePositions2[4] = scale * -1.0f;  // Bottom left
	smallRectanglePositions2[5] = scale * -0.3f;
	smallRectanglePositions2[6] = scale * -1.0f;  // Top left
	smallRectanglePositions2[7] = scale * -0.2f;

	initialSmallRectanglePositions2[0] = smallRectanglePositions2[0];
	initialSmallRectanglePositions2[1] = smallRectanglePositions2[1];
	initialSmallRectanglePositions2[2] = smallRectanglePositions2[2];
	initialSmallRectanglePositions2[3] = smallRectanglePositions2[3];
	initialSmallRectanglePositions2[4] = smallRectanglePositions2[4];
	initialSmallRectanglePositions2[5] = smallRectanglePositions2[5];
	initialSmallRectanglePositions2[6] = smallRectanglePositions2[6];
	initialSmallRectanglePositions2[7] = smallRectanglePositions2[7];

    // Translate the entire geometry to center
    TranslateToCenter(circlePositions, numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(rectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(squarePositions, numSquareVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(rightTrianglePositions1, numRightTriangleVertices1, -scale * 0.4f, 0.0f);
	TranslateToCenter(rightTrianglePositions2, numRightTriangleVertices2, -scale * 0.4f, 0.0f);
	TranslateToCenter(rightTrianglePositions3, numRightTriangleVertices3, -scale * 0.4f, 0.0f);
	TranslateToCenter(rightTrianglePositions4, numRightTriangleVertices4, -scale * 0.4f, 0.0f);
	TranslateToCenter(smallRectanglePositions1, numSmallRectangleVertices1, -scale * 0.4f, 0.0f);
	TranslateToCenter(smallRectanglePositions2, numSmallRectangleVertices2, -scale * 0.4f, 0.0f);

    TranslateToCenter(initialCirclePositions, numCircleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(initialRectanglePositions, numRectangleVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(initialSquarePositions, numSquareVertices, -scale * 0.4f, 0.0f);
    TranslateToCenter(initialRightTrianglePositions1, numRightTriangleVertices1, -scale * 0.4f, 0.0f);
	TranslateToCenter(initialRightTrianglePositions2, numRightTriangleVertices2, -scale * 0.4f, 0.0f);
	TranslateToCenter(initialRightTrianglePositions3, numRightTriangleVertices3, -scale * 0.4f, 0.0f);
	TranslateToCenter(initialRightTrianglePositions4, numRightTriangleVertices4, -scale * 0.4f, 0.0f);
	TranslateToCenter(initialSmallRectanglePositions1, numSmallRectangleVertices1, -scale * 0.4f, 0.0f);
	TranslateToCenter(initialSmallRectanglePositions2, numSmallRectangleVertices2, -scale * 0.4f, 0.0f);

    index = 0;

    // Left half-circle indices
    for (int i = 0; i < 20; ++i) {
        circleIndices[index++] = 0;
        circleIndices[index++] = i;
        circleIndices[index++] = i + 1;
    }

    // Rectangle indices
    rectangleIndices[0] = 0; // Top right
    rectangleIndices[1] = 1; // Bottom right
    rectangleIndices[2] = 2; // Bottom left
    rectangleIndices[3] = 0; // Top right
    rectangleIndices[4] = 2; // Bottom left
    rectangleIndices[5] = 3; // Top left

    // Square indices
    squareIndices[0] = 0; // Top right
    squareIndices[1] = 1; // Bottom right
    squareIndices[2] = 2; // Bottom left
    squareIndices[3] = 0; // Top right
    squareIndices[4] = 2; // Bottom left
    squareIndices[5] = 3; // Top left

    // Right triangle1 indices
    rightTriangleIndices1[0] = 0; // Bottom right
    rightTriangleIndices1[1] = 1; // Bottom left
    rightTriangleIndices1[2] = 2; // Top left

	// Right triangle2 indices
	rightTriangleIndices2[0] = 0; // Top right
	rightTriangleIndices2[1] = 1; // Top left
	rightTriangleIndices2[2] = 2; // Bottom left

	// Right triangle3 indices
	rightTriangleIndices3[0] = 0; // Top right
	rightTriangleIndices3[1] = 1; // Bottom right
	rightTriangleIndices3[2] = 2; // Bottom left

	// Right triangle4 indices 
	rightTriangleIndices4[0] = 0; // Bottom right
	rightTriangleIndices4[1] = 1; // Bottom left
	rightTriangleIndices4[2] = 2; // Top left

	// Small rectangle1 indices
	smallRectangleIndices1[0] = 0; // Top right
	smallRectangleIndices1[1] = 1; // Bottom right
	smallRectangleIndices1[2] = 2; // Bottom left
	smallRectangleIndices1[3] = 0; // Top right
	smallRectangleIndices1[4] = 2; // Bottom left
	smallRectangleIndices1[5] = 3; // Top left

	// Small rectangle2 indices
	smallRectangleIndices2[0] = 0; // Top right
	smallRectangleIndices2[1] = 1; // Bottom right
	smallRectangleIndices2[2] = 2; // Bottom left
	smallRectangleIndices2[3] = 0; // Top right
	smallRectangleIndices2[4] = 2; // Bottom left
	smallRectangleIndices2[5] = 3; // Top left

    // Vertex Array Object and Buffer for circles
    GLCall(glGenVertexArrays(1, &circleVao));
    GLCall(glGenBuffers(1, &circleVbo));    
    GLCall(glGenBuffers(1, &circleEbo));

    // Initialize VAO and VBO for circles
    GLCall(glBindVertexArray(circleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numCircleVertices * 2, circlePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, circleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numCircleIndices, circleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Vertex Array Object and Buffer for rectangle
    GLCall(glGenVertexArrays(1, &rectangleVao));
    GLCall(glGenBuffers(1, &rectangleVbo));
    GLCall(glGenBuffers(1, &rectangleEbo));

    // Initialize VAO and VBO for rectangle
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numRectangleVertices * 2, rectanglePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rectangleEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numRectangleIndices, rectangleIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Vertex Array Object and Buffer for square
    GLCall(glGenVertexArrays(1, &squareVao));
    GLCall(glGenBuffers(1, &squareVbo));
    GLCall(glGenBuffers(1, &squareEbo));

    // Initialize VAO and VBO for square
    GLCall(glBindVertexArray(squareVao));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, squareVbo));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numSquareVertices * 2, squarePositions, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, squareEbo));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numSquareIndices, squareIndices, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

    // Vertex Array Object and Buffer for right triangle1
    GLCall(glGenVertexArrays(1, &rightTriangleVao1));
    GLCall(glGenBuffers(1, &rightTriangleVbo1));
    GLCall(glGenBuffers(1, &rightTriangleEbo1));

    // Initialize VAO and VBO for right triangle1
    GLCall(glBindVertexArray(rightTriangleVao1));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo1));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rightTriangleEbo1));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numRightTriangleIndices1, rightTriangleIndices1, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

	// Vertex Array Object and Buffer for right triangle2
	GLCall(glGenVertexArrays(1, &rightTriangleVao2));
	GLCall(glGenBuffers(1, &rightTriangleVbo2));
    GLCall(glGenBuffers(1, &rightTriangleEbo2));

    // Initialize VAO and VBO for right triangle2
    GLCall(glBindVertexArray(rightTriangleVao2));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo2));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rightTriangleEbo2));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int) * numRightTriangleIndices2, rightTriangleIndices2, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));

	// Vertex Array Object and Buffer for right triangle3
	GLCall(glGenVertexArrays(1, &rightTriangleVao3));
	GLCall(glGenBuffers(1, &rightTriangleVbo3));
	GLCall(glGenBuffers(1, &rightTriangleEbo3));

	// Initialize VAO and VBO for right triangle3
	GLCall(glBindVertexArray(rightTriangleVao3));
	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo3));
	GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float)* numRightTriangleVertices3 * 2, rightTrianglePositions3, GL_DYNAMIC_DRAW));

	GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rightTriangleEbo3));
	GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int)* numRightTriangleIndices3, rightTriangleIndices3, GL_STATIC_DRAW));

	GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
	GLCall(glEnableVertexAttribArray(0));

	// Vertex Array Object and Buffer for right triangle4
	GLCall(glGenVertexArrays(1, &rightTriangleVao4));
	GLCall(glGenBuffers(1, &rightTriangleVbo4));
	GLCall(glGenBuffers(1, &rightTriangleEbo4));

	// Initialize VAO and VBO for right triangle4
	GLCall(glBindVertexArray(rightTriangleVao4));
	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo4));
	GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float)* numRightTriangleVertices4 * 2, rightTrianglePositions4, GL_DYNAMIC_DRAW));

	GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, rightTriangleEbo4));
	GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int)* numRightTriangleIndices4, rightTriangleIndices4, GL_STATIC_DRAW));

	GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
	GLCall(glEnableVertexAttribArray(0));

	// Vertex Array Object and Buffer for small rectangle1
	GLCall(glGenVertexArrays(1, &smallRectangleVao1));
	GLCall(glGenBuffers(1, &smallRectangleVbo1));
	GLCall(glGenBuffers(1, &smallRectangleEbo1));

	// Initialize VAO and VBO for small rectangle1
	GLCall(glBindVertexArray(smallRectangleVao1));
	GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo1));
	GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float)* numSmallRectangleVertices1 * 2, smallRectanglePositions1, GL_DYNAMIC_DRAW));

	GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, smallRectangleEbo1));
	GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int)* numSmallRectangleIndices1, smallRectangleIndices1, GL_STATIC_DRAW));

	GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
	GLCall(glEnableVertexAttribArray(0));

	// Vertex Array Object and Buffer for small rectangle2
	GLCall(glGenVertexArrays(1, &smallRectangleVao2));
	GLCall(glGenBuffers(1, &smallRectangleVbo2));
	GLCall(glGenBuffers(1, &smallRectangleEbo2));

    // Initialize VAO and VBO for small rectangle2
    GLCall(glBindVertexArray(smallRectangleVao2));
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo2));
    GLCall(glBufferData(GL_ARRAY_BUFFER, sizeof(float)* numSmallRectangleVertices2 * 2, smallRectanglePositions2, GL_DYNAMIC_DRAW));

    GLCall(glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, smallRectangleEbo2));
    GLCall(glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(unsigned int)* numSmallRectangleIndices2, smallRectangleIndices2, GL_STATIC_DRAW));

    GLCall(glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 2 * sizeof(float), nullptr));
    GLCall(glEnableVertexAttribArray(0));
}


void RobotArmEndEffector::UpdateRotation(float angle, float centerX, float centerY) {
    for (int i = 0; i < numCircleVertices * 2; i += 2) {
        float x = initialCirclePositions[i] - centerX;
        float y = initialCirclePositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        circlePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        circlePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }

    for (int i = 0; i < numRectangleVertices * 2; i += 2) {
        float x = initialRectanglePositions[i] - centerX;
        float y = initialRectanglePositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        rectanglePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        rectanglePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }

    for (int i = 0; i < numSquareVertices * 2; i += 2) {
        float x = initialSquarePositions[i] - centerX;
        float y = initialSquarePositions[i + 1] - centerY;

        // Apply rotation (negate angle for clockwise rotation)
        squarePositions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
        squarePositions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
    }

	for (int i = 0; i < numRightTriangleVertices1 * 2; i += 2) {
		float x = initialRightTrianglePositions1[i] - centerX;
		float y = initialRightTrianglePositions1[i + 1] - centerY;

		// Apply rotation (negate angle for clockwise rotation)
		rightTrianglePositions1[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		rightTrianglePositions1[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

	for (int i = 0; i < numRightTriangleVertices2 * 2; i += 2) {
		float x = initialRightTrianglePositions2[i] - centerX;
		float y = initialRightTrianglePositions2[i + 1] - centerY;
        
		// Apply rotation (negate angle for clockwise rotation)
		rightTrianglePositions2[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		rightTrianglePositions2[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

	for (int i = 0; i < numRightTriangleVertices3 * 2; i += 2) {
		float x = initialRightTrianglePositions3[i] - centerX;
		float y = initialRightTrianglePositions3[i + 1] - centerY;
		// Apply rotation (negate angle for clockwise rotation)
		rightTrianglePositions3[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		rightTrianglePositions3[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

	for (int i = 0; i < numRightTriangleVertices4 * 2; i += 2) {
		float x = initialRightTrianglePositions4[i] - centerX;
		float y = initialRightTrianglePositions4[i + 1] - centerY;
		// Apply rotation (negate angle for clockwise rotation)
		rightTrianglePositions4[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		rightTrianglePositions4[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

	for (int i = 0; i < numSmallRectangleVertices1 * 2; i += 2) {
		float x = initialSmallRectanglePositions1[i] - centerX;
		float y = initialSmallRectanglePositions1[i + 1] - centerY;

		// Apply rotation (negate angle for clockwise rotation)
		smallRectanglePositions1[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		smallRectanglePositions1[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

	for (int i = 0; i < numSmallRectangleVertices2 * 2; i += 2) {
		float x = initialSmallRectanglePositions2[i] - centerX;
		float y = initialSmallRectanglePositions2[i + 1] - centerY;

		// Apply rotation (negate angle for clockwise rotation)
		smallRectanglePositions2[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
		smallRectanglePositions2[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
	}

    // Update the vertex buffer with the new positions
    GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
    GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numCircleVertices * 2, circlePositions));

    GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
    GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRectangleVertices * 2, rectanglePositions));

    GLCall(glBindBuffer(GL_ARRAY_BUFFER, squareVbo));
    GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSquareVertices * 2, squarePositions));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo1));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo2));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo3));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices3 * 2, rightTrianglePositions3));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo4));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices4 * 2, rightTrianglePositions4));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo1));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSmallRectangleVertices1 * 2, smallRectanglePositions1));

	GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo2));
	GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSmallRectangleVertices2 * 2, smallRectanglePositions2));
}


void RobotArmEndEffector::TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
    // Update the vertex buffer with the new positions
    if (positions == circlePositions) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, circleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numCircleVertices * 2, circlePositions));
    }
    else if (positions == rectanglePositions) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, rectangleVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRectangleVertices * 2, rectanglePositions));
    }
    else if (positions == squarePositions) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, squareVbo));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSquareVertices * 2, squarePositions));
	}
    else if (positions == rightTrianglePositions1) {
        GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo1));
        GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices1 * 2, rightTrianglePositions1));
    }
	else if (positions == rightTrianglePositions2) {
		GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo2));
		GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices2 * 2, rightTrianglePositions2));
	}
	else if (positions == rightTrianglePositions3) {
		GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo3));
		GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices3 * 2, rightTrianglePositions3));
	}
	else if (positions == rightTrianglePositions4) {
		GLCall(glBindBuffer(GL_ARRAY_BUFFER, rightTriangleVbo4));
		GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numRightTriangleVertices4 * 2, rightTrianglePositions4));
	}
	else if (positions == smallRectanglePositions1) {
		GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo1));
		GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSmallRectangleVertices1 * 2, smallRectanglePositions1));
	}
	else if (positions == smallRectanglePositions2) {
		GLCall(glBindBuffer(GL_ARRAY_BUFFER, smallRectangleVbo2));
		GLCall(glBufferSubData(GL_ARRAY_BUFFER, 0, sizeof(float) * numSmallRectangleVertices2 * 2, smallRectanglePositions2));
	}
}

void RobotArmEndEffector::TranslateToPosition(float x, float y) {
    // Calculate the offset needed to translate the appendage to the specified position
    auto redDotPosition = CalculateRedDotPosition();
    float offsetX = x - redDotPosition.first;
    float offsetY = y - redDotPosition.second;

    // Translate the entire geometry
    TranslateArbitrary(circlePositions, numCircleVertices, offsetX, offsetY);
    TranslateArbitrary(rectanglePositions, numRectangleVertices, offsetX, offsetY);
    TranslateArbitrary(squarePositions, numSquareVertices, offsetX, offsetY);
	TranslateArbitrary(rightTrianglePositions1, numRightTriangleVertices1, offsetX, offsetY);
	TranslateArbitrary(rightTrianglePositions2, numRightTriangleVertices2, offsetX, offsetY);
	TranslateArbitrary(rightTrianglePositions3, numRightTriangleVertices3, offsetX, offsetY);
	TranslateArbitrary(rightTrianglePositions4, numRightTriangleVertices4, offsetX, offsetY);
	TranslateArbitrary(smallRectanglePositions1, numSmallRectangleVertices1, offsetX, offsetY);
	TranslateArbitrary(smallRectanglePositions2, numSmallRectangleVertices2, offsetX, offsetY);
}

void RobotArmEndEffector::Render(const Shader& shader) {
    int location = shader.GetUniformLocation("u_Color");

    // Draw the outline
    GLCall(glLineWidth(2.0f)); // Set the line width for the outline
    GLCall(glUniform4f(location, 0.0f, 0.0f, 0.0f, 1.0f)); // Set color to black

    // Draw the circles outline
    GLCall(glBindVertexArray(circleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle outline
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glDrawElements(GL_LINE_LOOP, numRectangleIndices, GL_UNSIGNED_INT, nullptr));

	// Draw the square outline
	GLCall(glBindVertexArray(squareVao));
	GLCall(glDrawElements(GL_LINE_LOOP, numSquareIndices, GL_UNSIGNED_INT, nullptr));

	// Draw small rectangle 1 outline
	GLCall(glBindVertexArray(smallRectangleVao1));
	GLCall(glDrawElements(GL_LINE_LOOP, numSmallRectangleIndices1, GL_UNSIGNED_INT, nullptr));

	// Draw small rectangle 2 outline
	GLCall(glBindVertexArray(smallRectangleVao2));
	GLCall(glDrawElements(GL_LINE_LOOP, numSmallRectangleIndices2, GL_UNSIGNED_INT, nullptr));

	// Draw right triangle 1 outline
	GLCall(glBindVertexArray(rightTriangleVao1));
	GLCall(glDrawElements(GL_LINE_LOOP, numRightTriangleIndices1, GL_UNSIGNED_INT, nullptr));

	// Draw right triangle 2 outline
	GLCall(glBindVertexArray(rightTriangleVao2));
	GLCall(glDrawElements(GL_LINE_LOOP, numRightTriangleIndices2, GL_UNSIGNED_INT, nullptr));

	// Draw right triangle 3 outline
	GLCall(glBindVertexArray(rightTriangleVao3));
	GLCall(glDrawElements(GL_LINE_LOOP, numRightTriangleIndices3, GL_UNSIGNED_INT, nullptr));

	// Draw right triangle 4 outline
	GLCall(glBindVertexArray(rightTriangleVao4));
	GLCall(glDrawElements(GL_LINE_LOOP, numRightTriangleIndices4, GL_UNSIGNED_INT, nullptr));

    // Draw the fill
    GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver

    // Draw the circles fill
    GLCall(glBindVertexArray(circleVao));
    GLCall(glDrawElements(GL_TRIANGLES, numCircleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the rectangle fill
    GLCall(glBindVertexArray(rectangleVao));
    GLCall(glDrawElements(GL_TRIANGLES, numRectangleIndices, GL_UNSIGNED_INT, nullptr));

    // Draw the square fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(squareVao));
    GLCall(glDrawElements(GL_TRIANGLES, numSquareIndices, GL_UNSIGNED_INT, nullptr));

	// Draw the right triangle1 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(rightTriangleVao1));
	GLCall(glDrawElements(GL_TRIANGLES, numRightTriangleIndices1, GL_UNSIGNED_INT, nullptr));

	// Draw the right triangle2 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(rightTriangleVao2));
	GLCall(glDrawElements(GL_TRIANGLES, numRightTriangleIndices2, GL_UNSIGNED_INT, nullptr));

	// Draw the right triangle3 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(rightTriangleVao3));
	GLCall(glDrawElements(GL_TRIANGLES, numRightTriangleIndices3, GL_UNSIGNED_INT, nullptr));

	// Draw the right triangle4 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(rightTriangleVao4));
	GLCall(glDrawElements(GL_TRIANGLES, numRightTriangleIndices4, GL_UNSIGNED_INT, nullptr));

	// Draw the small rectangle1 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(smallRectangleVao1));
	GLCall(glDrawElements(GL_TRIANGLES, numSmallRectangleIndices1, GL_UNSIGNED_INT, nullptr));

	// Draw the small rectangle2 fill
	GLCall(glUniform4f(location, 0.75f, 0.75f, 0.75f, 1.0f)); // Set color to metallic gray/silver
	GLCall(glBindVertexArray(smallRectangleVao2));
	GLCall(glDrawElements(GL_TRIANGLES, numSmallRectangleIndices2, GL_UNSIGNED_INT, nullptr));
}

std::pair<float, float> RobotArmEndEffector::CalculateRectangleHeightMidpoint() const {
    // Midpoint of the rectangle's height on the left side
    float midpointX = (rectanglePositions[0] + rectanglePositions[2]) / 2;
    float midpointY = (rectanglePositions[1] + rectanglePositions[3]) / 2;
    return { midpointX, midpointY };
}

void RobotArmEndEffector::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

std::pair<float, float> RobotArmEndEffector::CalculateRedDotPosition() const {
    // Calculate the red dot position based on the rectangle's height midpoint
    return CalculateRectangleHeightMidpoint();
}
