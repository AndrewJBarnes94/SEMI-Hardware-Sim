#include "Renderer.h"

Renderer::Renderer(int width, int height, const char* title)
    : window(nullptr), buffer(0), windowWidth(width), windowHeight(height), windowTitle(title)
{
	if (!initialize()) {
		std::cerr << "Renderer initialization failed!" << std::endl;
	}
}

unsigned int Renderer::CompileShader(unsigned int type, const std::string& source)
{
    unsigned int id = glCreateShader(type);
    const char* src = source.c_str();
    glShaderSource(id, 1, &src, nullptr);
    glCompileShader(id);

    // Error handling
    int result;
    glGetShaderiv(id, GL_COMPILE_STATUS, &result);
    if (result == GL_FALSE) {
        int length;
        glGetShaderiv(id, GL_INFO_LOG_LENGTH, &length);
        char* message = (char*)alloca(length * sizeof(char));
        glGetShaderInfoLog(id, length, &length, message);
        std::cerr << "Failed to compile " << (type == GL_VERTEX_SHADER ? "vertex" : "fragment") << " shader!" << std::endl;
        std::cerr << message << std::endl;
        glDeleteShader(id);
        return 0;
    }
    return id;
}


unsigned int Renderer::CreateShader(const std::string& vertexShader, const std::string& fragmentShader)
{
    unsigned int program = glCreateProgram();
    unsigned int vs = CompileShader(GL_VERTEX_SHADER, vertexShader);
    unsigned int fs = CompileShader(GL_FRAGMENT_SHADER, fragmentShader);

    if (vs == 0 || fs == 0) {
        glDeleteShader(vs);
        glDeleteShader(fs);
        return 0; // Shader compilation failed
    }

    glAttachShader(program, vs);
    glAttachShader(program, fs);
    glLinkProgram(program);
    glValidateProgram(program);

    glDeleteShader(vs);
    glDeleteShader(fs);

    return program;
}


bool Renderer::initialize() {
    if (!glfwInit()) {
        std::cerr << "Failed to initialize GLFW." << std::endl;
        return false;
    }

    window = glfwCreateWindow(windowWidth, windowHeight, windowTitle, NULL, NULL);
    if (!window) {
        std::cerr << "Failed to create GLFW window." << std::endl;
        glfwTerminate();
        return false;
    }

    glfwMakeContextCurrent(window);

    if (glewInit() != GLEW_OK) {
        std::cerr << "Failed to initialize GLEW." << std::endl;
        return false;
    }

    std::cout << "OpenGL Version: " << glGetString(GL_VERSION) << std::endl;
    return true;
}

void Renderer::setup() {
    float positions[6] = {
        -0.5f, -0.5f,
         0.0f,  0.5f,
         0.5f, -0.5f
    };

    unsigned int vao;
    glGenVertexArrays(1, &vao);
    glBindVertexArray(vao);

    glGenBuffers(1, &buffer);
    glBindBuffer(GL_ARRAY_BUFFER, buffer);
    glBufferData(GL_ARRAY_BUFFER, sizeof(positions), positions, GL_STATIC_DRAW);
    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, sizeof(float) * 2, 0);
    glEnableVertexAttribArray(0);

    std::string vertexShader =
        "#version 330 core\n"
        "layout(location = 0) in vec4 position;\n"
        "void main() {\n"
        "   gl_Position = position;\n"
        "}\n";

    std::string fragmentShader =
        "#version 330 core\n"
        "layout(location = 0) out vec4 color;\n"
        "void main() {\n"
        "   color = vec4(1.0, 0.0, 0.0, 1.0);\n"
        "}\n";

    unsigned int shader = CreateShader(vertexShader, fragmentShader);
    if (shader == 0) {
        std::cerr << "Shader program creation failed!" << std::endl;
        return;
    }
    glUseProgram(shader);
}


void Renderer::renderLoop() {
    while (!glfwWindowShouldClose(window)) {
        glClear(GL_COLOR_BUFFER_BIT);

        glDrawArrays(GL_TRIANGLES, 0, 3);

        glfwSwapBuffers(window);
        glfwPollEvents();
    }
}

void Renderer::cleanup() {
    glDeleteBuffers(1, &buffer);
    glfwTerminate();
}

Renderer::~Renderer() {
    cleanup();
}