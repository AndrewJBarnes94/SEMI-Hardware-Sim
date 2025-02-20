// File: main.cpp
#include <WinSock2.h>
#include <GL/glew.h>
#include <GLFW/glfw3.h>
#include <iostream>
#include <fstream>
#include <atomic>
#include <map>
#include <thread>
#include <windows.h>
#include "Shader.h"
#include "Chamber/Chamber.h"
#include "Chamber/ProcessModule.h"
#include "InputDrivenComponents/SlitValve.h"
#include "Chamber/Loadport.h"
#include "Robot/RobotArm.h"
#include "ErrorHandling.h"
#include "Server.h"
#include <tlhelp32.h>
#include <string>
#include <thread>

// Ensure platform-specific functions are available
#define GLFW_EXPOSE_NATIVE_WIN32  
#include <GLFW/glfw3native.h>

// Function to attach a console in debug mode
void AttachConsoleIfDebug() {
#ifdef _DEBUG  // Only create a console in debug mode
    if (AllocConsole()) {
        FILE* stream;
        freopen_s(&stream, "CONOUT$", "w", stdout);
        freopen_s(&stream, "CONOUT$", "w", stderr);
        freopen_s(&stream, "CONIN$", "r", stdin);
        std::cout << "Debug console attached!" << std::endl;
    }
#endif
}

std::atomic<float> angle1(0.0f);
std::atomic<float> angle2(0.0f);
std::atomic<float> angle3(0.0f);
std::atomic<bool> newInputReceived(false);

// Function to get HWND from GLFW window
HWND GetOpenGLWindowHandle(GLFWwindow* window) {
    return glfwGetWin32Window(window);
}

// **Non-blocking wait for C# to attach window**
void waitForEmbedding(HWND hWnd) {
    int timeout = 20;  // Wait for up to 2 seconds (100ms * 20)
    std::cout << "Waiting for C# application to embed OpenGL window...\n";

    while (timeout-- > 0) {
        if (GetParent(hWnd) != NULL) {  // C# has attached it
            std::cout << "Embedded! Showing OpenGL window.\n";
            //ShowWindow(hWnd, SW_SHOW);
            return;
        }
        std::this_thread::sleep_for(std::chrono::milliseconds(100));  // Shorter wait time
    }

    std::cerr << "Warning: C# did not attach OpenGL window in time. Running standalone.\n";
    ShowWindow(hWnd, SW_SHOW);  // Show anyway if C# does not attach
}

float UpdateRotation(float x, float y, float initialX, float initialY, float angle, float centerX, float centerY) {
    float xDiff = initialX - centerX;
    float yDiff = initialY - centerY;
    // Apply rotation (negate angle for clockwise rotation)
    x = cos(-angle) * xDiff - sin(-angle) * yDiff + centerX; // New x
    y = sin(-angle) * xDiff + cos(-angle) * yDiff + centerY; // New y
    return x;
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    AttachConsoleIfDebug();  // Attach console only in Debug mode

    AttachConsoleIfDebug();  // Attach console only in Debug mode

    //HWND hwnd = GetConsoleWindow();
    //ShowWindow(hwnd, SW_SHOW);  // Hide the console

    // Initialize GLFW
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return -1;
    }

    // Create a **hidden** OpenGL window (for embedding)
    glfwWindowHint(GLFW_VISIBLE, GLFW_FALSE);
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

    HWND hWnd = GetOpenGLWindowHandle(window);

    // **Start non-blocking wait in a separate thread**
    waitForEmbedding(hWnd);

    // Show the OpenGL window and ensure rendering
    glfwShowWindow(window);
    glfwMakeContextCurrent(window);
    glfwSwapBuffers(window);
    glfwPollEvents();

    // Load Shader
    Shader shader("res/shaders/basic.shader");
    shader.Bind();

    float masterScale = 0.5f;

    // Initialize Chamber
    Chamber chamber(0.475f * masterScale);
    chamber.Initialize();
    chamber.getPositionMap("center");

    // Initialize Robot Arm
    RobotArm robotArm(angle1, angle2, angle3, newInputReceived, 0.3f * masterScale);
    robotArm.Initialize(90.0f, 90.0f, 45.0f);

    Loadport loadport1(0.5f * masterScale);
    loadport1.Initialize();

    float slitValveTRx = -0.515f;   // top right x-value
    float slitValveTRy = 0.3f;      // top right y-value
	float slitValveBRx = -0.515f;     // bottom right x-value
	float slitValveBRy = -0.3f;      // bottom right y-value
	float slitValveBLx = -0.6f;      // bottom left x-value
	float slitValveBLy = -0.3f;      // bottom left y-value
	float slitValveTLx = -0.6f;      // top left x-value
	float slitValveTLy = 0.3f;       // top left y-value


    ProcessModule pm1(
        0.8f * masterScale,
        -0.6f, 0.3f,
        -0.6f, -0.3f,
        -1.1f, -0.3f,
        -1.1f, 0.3f
    );
    pm1.Initialize();

    SlitValve slitValve1(
        0.8f * masterScale,
        slitValveTRx, slitValveTRy,
        slitValveBRx, slitValveBRy,
        slitValveBLx, slitValveBLy,
        slitValveTLx, slitValveTLy
    );
    slitValve1.Initialize();

    ProcessModule pm2(
        0.8f * masterScale,
        -0.040f, 0.670f,
        -0.560f, 0.370f,
        -0.810f, 0.803f,
        -0.290f, 1.103f
    );
    pm2.Initialize();

    SlitValve slitValve2(
        0.8f * masterScale,
        0.002f, 0.596f,
        -0.517f, 0.296f,
        -0.560f, 0.370f,
        -0.040f, 0.670f
    );
    slitValve2.Initialize();

    ProcessModule pm3(
        0.8f * masterScale,
        0.560f, 0.370f,
        0.040f, 0.670f,
        0.290f, 1.103f,
        0.810f, 0.803f
    );
    pm3.Initialize();

    SlitValve slitValve3(
        0.8f * masterScale,
        0.517f, 0.296f,
        -0.002f, 0.596f,
        0.040f, 0.670f,
        0.560f, 0.370f
    );
    slitValve3.Initialize();

    ProcessModule pm4(
        0.8f * masterScale,
        0.600f, -0.300f,
        0.600f, 0.300f,
        1.100f, 0.300f,
        1.100f, -0.300f
    );
    pm4.Initialize();

    SlitValve slitValve4(
        0.8f * masterScale,
        0.515f, -0.300f,
        0.515f, 0.300f,
        0.600f, 0.300f,
        0.600f, -0.300f
    );
    slitValve4.Initialize();

    SlitValve slitValve5(
        0.8f * masterScale,
        -0.002f, -0.714f,
        0.517f, -0.489f,
        0.560f, -0.563f,
        0.040f, -0.789f
    );
    slitValve5.Initialize();

    SlitValve slitValve6(
        0.8f * masterScale,
        0.002f, -0.744f,
        -0.517f, -0.519f,
        -0.560f, -0.593f,
        -0.040f, -0.819f
    );
    slitValve6.Initialize();



    // Start the server
    auto server = std::make_shared<Server>(12345, angle1, angle2, angle3, newInputReceived);
    server->Start();
    std::cout << "Server started on port 12345" << std::endl;

    while (!glfwWindowShouldClose(window)) {
        GLCall(glClear(GL_COLOR_BUFFER_BIT));

        if (newInputReceived) {
            std::cout << "New input received: angle1 = " << angle1 << ", angle2 = " << angle2 << ", angle3 = " << angle3 << std::endl;
            robotArm.Update();
            newInputReceived = false;
        }

        // Render Chamber with a specific point
        chamber.Render(shader);

        // Render Robot Arm
        robotArm.Render();

        // Render Loadport 1
        //loadport1.Render(shader);

        pm1.Render(shader);
        slitValve1.Render(shader);
        pm2.Render(shader);
        slitValve2.Render(shader);
        pm3.Render(shader);
        slitValve3.Render(shader);
        pm4.Render(shader);
        slitValve4.Render(shader);

        //slitValve5.Render(shader);
        //slitValve6.Render(shader);

        server->Poll(); // Poll the io_context to handle asynchronous operations

        GLCall(glfwSwapBuffers(window));
        GLCall(glfwPollEvents());
    }

    GLCall(glfwTerminate());
    server->Stop();
    std::cout << "Server stopped" << std::endl;
    return 0;
}
