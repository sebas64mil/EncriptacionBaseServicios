using UnityEngine;
using TMPro;
using System;

public class TCPServerUI : MonoBehaviour
{
    public int serverPort = 5555;
    [SerializeField] private TCPServer serverReference;

    // Ya no usamos un input de texto; obtenemos el hash desde HashManager
    [SerializeField] private HashManager hashManager;

    // Referencia al EncryptManager para crear el payload firmado
    [SerializeField] private EncryptManager encryptManager;

    private IServer _server;
    void Awake()
    {
        _server = serverReference;
    }
    void Start()
    {
        _server.OnMessageReceived += HandleMessageReceived;
        _server.OnConnected += HandleConnection;
        _server.OnDisconnected += HandleDisconnection;
    }
    public void StartServer()
    {
        _server.StartServer(serverPort);
    }
    public void SendServerMessage()
    {
        if (!_server.isServerRunning)
        {
            Debug.Log("[UI-Server] El servidor no está en ejecución");
            return;
        }

        if (hashManager == null)
        {
            Debug.LogWarning("[UI-Server] hashManager no asignado en el inspector");
            return;
        }

        string hashHex = hashManager.GetHash();
        if (string.IsNullOrEmpty(hashHex))
        {
            Debug.Log("[UI-Server] No hay hash generado para enviar");
            return;
        }

        if (encryptManager == null)
        {
            Debug.LogWarning("[UI-Server] encryptManager no asignado en el inspector");
            return;
        }

        string payload;
        try
        {
            payload = encryptManager.CreateSignedPayload(hashHex);
        }
        catch (Exception ex)
        {
            Debug.Log("[UI-Server] Error creando payload firmado: " + ex.Message);
            return;
        }

        _server.SendMessageAsync(payload);
        Debug.Log("[UI-Server] Payload enviado: " + payload);
    }

    void HandleMessageReceived(string text)
    {
        Debug.Log("[UI-Server] Message received from client: " + text);
    }

    void HandleConnection()
    {
        Debug.Log("[UI-Server] Client Connected to Server");
    }
    void HandleDisconnection()
    {
        Debug.Log("[UI-Server] Client Disconnect from Server");
    }
}