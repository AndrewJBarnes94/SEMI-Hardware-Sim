#pragma once
#ifndef ERROR_HANDLING_H
#define ERROR_HANDLING_H

#include <iostream>
#include <GL/glew.h>

class ErrorHandling {
public:
    static void ClearErrors() {
        while (glGetError() != GL_NO_ERROR);
    }

    static bool LogCall(const char* function, const char* file, int line) {
        while (GLenum error = glGetError()) {
            std::cerr << "[OpenGL Error] (" << error << ") in " << function
                << " at " << file << ":" << line << std::endl;
            return false;
        }
        return true;
    }
};

#define ASSERT(x) if (!(x)) __debugbreak();
#define GLCall(x) \
    ErrorHandling::ClearErrors(); \
    x; \
    ASSERT(ErrorHandling::LogCall(#x, __FILE__, __LINE__))

#endif
