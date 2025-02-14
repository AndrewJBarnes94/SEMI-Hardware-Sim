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

def pickAL():
    start_angle1 = 35.0
    start_angle2 = -35.0
    start_angle3 = -90.0

    num_steps = 45
    increment = 0.5  # Increase each angle by 1 degree per step
    delay = 0.03

    for i in range(num_steps):
        angle1 = start_angle1 + (i * increment)
        angle2 = start_angle2 + (i * increment)
        angle3 = start_angle3 + (i * increment)

        send_command(angle1, angle2, angle3)
        time.sleep(delay)

    for i in range(30):
        angle1 = angle1 - (i * increment * 0.5)
        angle2 = angle2 + (i * increment * 0.55)

        send_command(angle1, angle2, angle3)
        time.sleep(delay)

    for i in range(30):
        angle1 = angle1 + (i * increment * 0.5)
        angle2 = angle2 - (i * increment * 0.55)

        send_command(angle1, angle2, angle3)
        time.sleep(delay)

    print("Command sequence completed.")


if __name__ == "__main__":
    pickAL()