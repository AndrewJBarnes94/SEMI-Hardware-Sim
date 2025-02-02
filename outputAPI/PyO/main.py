import socket
import time

def send_command(angle1, angle2, angle3):
    host = '127.0.0.1'  # Localhost
    port = 12345        # Port to connect to

    command = f"{angle1} {angle2} {angle3}"  # Format the command as a string
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((host, port))
        s.sendall(command.encode())
        print(f"Sent: {command}")

if __name__ == "__main__":
    # Initial angles
    start_angle1 = 35.0
    start_angle2 = -35.0
    start_angle3 = -90.0

    # Number of iterations
    num_steps = 360
    increment = 1.0  # Increase each angle by 1 degree per step
    delay = 0.05  # 10 milliseconds

    for i in range(num_steps):
        # Calculate new angles
        angle1 = start_angle1 + (i * increment)
        angle2 = start_angle2 + (i * increment)
        angle3 = start_angle3 + (i * increment)

        # Send the command
        send_command(angle1, angle2, angle3)

        # Wait before sending the next command
        time.sleep(delay)

    print("Command sequence completed.")
