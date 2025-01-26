#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <thread>
#include <atomic>
#include "Shader.h"
#include "RobotArm.h"
#include "ErrorHandling.h"

std::atomic<float> angle1(0.0f); // Initial angle for the first appendage
std::atomic<float> angle2(0.0f); // Initial angle for the second appendage
std::atomic<bool> newInputReceived(false); // Flag to indicate new input

void HandleUserInput() {
    while (true) {
        float degree1, degree2;
        std::cin >> degree1;
        std::cin >> degree2;

        const float M_PI = 3.14159265358979323846f;

        float newAngle1 = degree1 * (M_PI / 180.0f); // Convert degree to radians
        float newAngle2 = degree2 * (M_PI / 180.0f); // Convert degree to radians

        angle1 = newAngle1;
        angle2 = newAngle2;
        newInputReceived = true; // Set the flag to indicate new input
    }
}

int main() {
    // Initialize GLFW
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return -1;
    }

    // Create window
    GLFWwindow* window = glfwCreateWindow(1000, 1500, "Rotating Arm", nullptr, nullptr);
    if (!window) {
        std::cerr << "Failed to create GLFW window." << std::endl;
        glfwTerminate();
        return -1;
    }

    glfwMakeContextCurrent(window);
    glfwSwapInterval(1); // Enable VSync

    // Initialize GLEW
    if (glewInit() != GLEW_OK) {
        std::cerr << "Failed to initialize GLEW." << std::endl;
        return -1;
    }

    std::cout << "OpenGL Version: " << glGetString(GL_VERSION) << std::endl;

    // Set the clear color to bluish gray (R, G, B, A)
    GLCall(glClearColor(0.2f, 0.3f, 0.4f, 1.0f));

    // Create and initialize RobotArm with a scaling factor of 0.5
    RobotArm robotArm(angle1, angle2, newInputReceived, 0.5f);
    robotArm.Initialize(0.0f, 0.0f, 45.0f); // Initialize with position (0, 0) and 45 degrees rotation

    // Start user input thread
    std::thread inputThread(HandleUserInput);

    // Main render loop
    while (!glfwWindowShouldClose(window)) {
        GLCall(glClear(GL_COLOR_BUFFER_BIT));

        robotArm.Update();
        robotArm.Render();

        GLCall(glfwSwapBuffers(window));
        GLCall(glfwPollEvents());
    }

    inputThread.join();
    GLCall(glfwTerminate());
    return 0;
}

