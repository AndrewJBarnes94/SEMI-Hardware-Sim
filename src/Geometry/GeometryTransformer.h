// File: GeometryTransformer.h
#pragma once

class GeometryTransformer {
public:
    static void TranslateArbitrary(float* positions, int numVertices, float offsetX, float offsetY);
    static void TranslateToCenter(float* positions, int numVertices, float offsetX, float offsetY);
    //static void UpdateRotation(float* positions, const float* initialPositions, int numVertices, float angle, float centerX, float centerY);
    static void UpdateRotation(float x, float y, float initialX, float initialY, float angle, float centerX, float centerY);
};
