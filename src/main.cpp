#include "Renderer.h"

int main() {
    Renderer renderer;

    if (!renderer.initialize()) {
        return -1;
    }

    renderer.setup();
    renderer.renderLoop();

    return 0;
}
