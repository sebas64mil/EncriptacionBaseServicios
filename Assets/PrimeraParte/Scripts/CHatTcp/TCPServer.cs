using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TCPServer : MonoBehaviour, IServer
{
    private TcpListener tcpListener; // TCP server declaration
    private TcpClient connectedClient; // Connected client declaration
    private NetworkStream networkStream; // Network data stream

    public bool isServerRunning { get; private set; }

    public event Action<string> OnMessageReceived;
    public event Action OnConnected;
    public event Action OnDisconnected;

    public async Task StartServer(int port)
    {
        tcpListener = new TcpListener(IPAddress.Any, port); // Configures the TCP server to listen on any IP and the specified port
        tcpListener.Start(); // Starts the TCP server

        Debug.Log("[Server] Server started, waiting for connections..."); // Displays a message in the Unity console indicating that the server has started
        isServerRunning = true;

        connectedClient = await tcpListener.AcceptTcpClientAsync(); //The server start listens for incoming client connections asynchronously
        Debug.Log("[Server] Client connected: " + connectedClient.Client.RemoteEndPoint);
        OnConnected?.Invoke(); // Invokes the OnConnected event, notifying any subscribed listeners that a client has connected

        networkStream = connectedClient.GetStream();
        _ = ReceiveLoop();
    }

    private async Task ReceiveLoop()
    {
        byte[] buffer = new byte[1024];// Buffer to store incoming data from the client 1024 bytes = 1 KB
        try
        {
            while (connectedClient != null && connectedClient.Connected)// Continuously checks if the client is still connected
            {
                int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);// Reads data from the network stream asynchronously and stores it in the buffer, returning the number of bytes read

                if (bytesRead == 0)//If the client is disconnected ReadAsync returns 0 bytes read
                {
                    Debug.Log("[Server] Client disconnected");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);// Converts the received bytes into a string message using UTF-8 encoding
                Debug.Log("[Server] Received: " + message);
                OnMessageReceived?.Invoke(message);// Invokes the OnMessageReceived event, passing the received message to any subscribed listeners
            }
        }
        finally
        {
            Disconnect();// Ensures that the connection is closed when the loop ends, whether due to disconnection or an error
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (networkStream == null || !connectedClient.Connected)// Checks if there is an active connection to a client before attempting to send a message
        {
            Debug.Log("[Server] No client connected");
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message);// Converts the message string into a byte array using UTF-8 encoding
        await networkStream.WriteAsync(data, 0, data.Length);// Writes the byte array to the network stream asynchronously, sending it to the connected client

        Debug.Log("[Server] Sent: " + message);
    }

    public void Disconnect() // Closes the connection to the client and cleans up resources
    {
        networkStream?.Close();
        connectedClient?.Close();

        networkStream = null;
        connectedClient = null;

        Debug.Log("[Server] Disconnected");
        OnDisconnected?.Invoke(); // Invokes the OnDisconnected event, notifying any subscribed listeners that the client has disconnected
    }

    private async void OnDestroy()
    {
        Disconnect();
        await Task.Delay(100);
    }
}
