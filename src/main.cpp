#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <thread>
#include <atomic>
#include "Shader.h"
#include "RobotArm.h"
#include "ErrorHandling.h"

std::atomic<float> rotationSpeed(0.0f); // Initial speed set to 0
std::atomic<bool> newInputReceived(false); // Flag to indicate new input

void HandleUserInput() {
    while (true) {
        float degree;
        std::cout << "Enter rotation degree for the arm (positive for clockwise, negative for counterclockwise): ";
        std::cin >> degree;

        const float M_PI = 3.14159265358979323846f;

        float speed = degree * (M_PI / 180.0f); // Convert degree to radians

        rotationSpeed = speed;
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
    GLFWwindow* window = glfwCreateWindow(800, 600, "Rotating Arm", nullptr, nullptr);
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
    RobotArm robotArm(rotationSpeed, newInputReceived, 0.5f);
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



