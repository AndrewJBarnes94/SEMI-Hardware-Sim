// File: Server.h
#pragma once

#include <asio.hpp>
#include <atomic>
#include <memory>

class Server : public std::enable_shared_from_this<Server> {
public:
    Server(int port, std::atomic<float>& angle1, std::atomic<float>& angle2, std::atomic<bool>& newInputReceived);
    ~Server();

    void Start();
    void Stop();
    void Poll();

private:
    void AcceptConnections();
    void HandleClient();

    asio::io_context io_context;
    asio::ip::tcp::acceptor acceptor;
    asio::ip::tcp::socket socket;
    std::atomic<float>& angle1;
    std::atomic<float>& angle2;
    std::atomic<bool>& newInputReceived;
    char data_[128]; // Declare the data_ member
};
