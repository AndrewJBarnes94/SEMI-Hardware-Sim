// File: GeometryTransformer.cpp
#include "GeometryTransformer.h"
#include <cmath>

void GeometryTransformer::TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

void GeometryTransformer::TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY) {
    for (int i = 0; i < numVertices * 2; i += 2) {
        positions[i] += offsetX;
        positions[i + 1] += offsetY;
    }
}

//void GeometryTransformer::UpdateRotation(float* positions, const float* initialPositions, int numVertices, float angle, float centerX, float centerY) {
//    for (int i = 0; i < numVertices * 2; i += 2) {
//        float x = initialPositions[i] - centerX;
//        float y = initialPositions[i + 1] - centerY;
//
//        // Apply rotation (negate angle for clockwise rotation)
//        positions[i] = cos(-angle) * x - sin(-angle) * y + centerX; // New x
//        positions[i + 1] = sin(-angle) * x + cos(-angle) * y + centerY; // New y
//    }
//}

void GeometryTransformer::UpdateRotation(float x, float y, float initialX, float initialY, float angle, float centerX, float centerY) {
	float xDiff = initialX - centerX;
	float yDiff = initialY - centerY;
	// Apply rotation (negate angle for clockwise rotation)
	x = cos(-angle) * xDiff - sin(-angle) * yDiff + centerX; // New x
	y = sin(-angle) * xDiff + cos(-angle) * yDiff + centerY; // New y
}
