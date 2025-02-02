import socket

def send_command(angle1, angle2):
    host = '127.0.0.1'  # Localhost
    port = 12345        # Port to connect to

    command = f"{angle1} {angle2}"  # Format the command as a string
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((host, port))
        s.sendall(command.encode())
        print(f"Sent: {command}")

if __name__ == "__main__":
    # Example commands
    send_command(45.0, 30.0)  # Update angles
    send_command(60.0, 45.0)  # Update angles
