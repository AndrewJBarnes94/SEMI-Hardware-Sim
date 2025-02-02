// File: main.cpp
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <atomic>
#include "Shader.h"
#include "RobotArm.h"
#include "ErrorHandling.h"
#include "Server.h"

std::atomic<float> angle1(0.0f);
std::atomic<float> angle2(0.0f);
std::atomic<bool> newInputReceived(false);

int main() {
    // Initialize GLFW
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return -1;
    }

    // Create window
    GLFWwindow* window = glfwCreateWindow(2000, 2000, "Robot Arm Control", nullptr, nullptr);
    if (!window) {
        std::cerr << "Failed to create GLFW window." << std::endl;
        glfwTerminate();
        return -1;
    }

    glfwMakeContextCurrent(window);
    glfwSwapInterval(1);

    // Initialize GLEW
    if (glewInit() != GLEW_OK) {
        std::cerr << "Failed to initialize GLEW." << std::endl;
        return -1;
    }

    std::cout << "OpenGL Version: " << glGetString(GL_VERSION) << std::endl;

    GLCall(glClearColor(0.2f, 0.3f, 0.4f, 1.0f));

    RobotArm robotArm(angle1, angle2, newInputReceived, 0.5f);
    robotArm.Initialize(90.0f, 90.0f, 45.0f);

    // Start the server
    auto server = std::make_shared<Server>(12345, angle1, angle2, newInputReceived);
    server->Start();
    std::cout << "Server started on port 12345" << std::endl;

    while (!glfwWindowShouldClose(window)) {
        GLCall(glClear(GL_COLOR_BUFFER_BIT));

        if (newInputReceived) {
            std::cout << "New input received: angle1 = " << angle1 << ", angle2 = " << angle2 << std::endl;
            robotArm.Update();
            newInputReceived = false;
        }

        robotArm.Render();

        server->Poll(); // Poll the io_context to handle asynchronous operations

        GLCall(glfwSwapBuffers(window));
        GLCall(glfwPollEvents());
    }

    GLCall(glfwTerminate());
    server->Stop();
    std::cout << "Server stopped" << std::endl;
    return 0;
}
