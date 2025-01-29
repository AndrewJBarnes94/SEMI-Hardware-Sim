#ifndef ROBOTARMENDEFFECTOR_H
#define ROBOTARMENDEFFECTOR_H

#include <GL/glew.h>
#include <atomic>

class EndEffector {
public:
    EndEffector(std::atomic<float>& angle, std::atomic<bool>& newInputReceived, float scale);
    void Initialize(float x, float y, float rotation);
    void Update();
    void Render();

private:
    std::atomic<float>& angle;
    std::atomic<bool>& newInputReceived;
    float scale;
    float x, y, rotation;
    GLuint VAO, VBO;

    void SetupGeometry();
};

#endif // ROBOTARMENDEFFECTOR_H
