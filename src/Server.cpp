#include "Server.h"
#include <iostream>
#include <sstream>

Server::Server(int port, std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<float>& angle3, std::atomic<bool>& newInputReceived)
    : acceptor(io_context, asio::ip::tcp::endpoint(asio::ip::tcp::v4(), port)),
    socket(io_context), angle1(angle1), angle2(angle2), angle3(angle3), newInputReceived(newInputReceived) {
    acceptor.set_option(asio::socket_base::reuse_address(true));
}

Server::~Server() {
    Stop();
}

void Server::Start() {
    AcceptConnections();
}

void Server::Stop() {
    io_context.stop();
}

void Server::Poll() {
    io_context.poll();
}

void Server::AcceptConnections() {
    acceptor.async_accept(socket, [this](std::error_code ec) {
        if (!ec) {
            std::cout << "Client connected" << std::endl;
            HandleClient();
        }
        AcceptConnections();
        });
}

void Server::HandleClient() {
    if (!socket.is_open()) {
        return;
    }

    auto self(shared_from_this());
    socket.async_read_some(asio::buffer(data_), [this, self](std::error_code error, std::size_t length) {
        if (error == asio::error::eof) {
            socket.close();
        }
        else if (error) {
            std::cerr << "Client error: " << error.message() << std::endl;
            socket.close();
        }
        else {
            std::string command(data_, length);
            std::istringstream iss(command);
            float newAngle1, newAngle2, newAngle3;
            if (iss >> newAngle1 >> newAngle2 >> newAngle3) {
                angle1 = newAngle1;
                angle2 = newAngle2;
                angle3 = newAngle3;
                newInputReceived = true;
                std::cout << "Received angles: " << newAngle1 << ", " << newAngle2 << ", " << newAngle3 << std::endl;
            }
            HandleClient();
        }
        });
}
