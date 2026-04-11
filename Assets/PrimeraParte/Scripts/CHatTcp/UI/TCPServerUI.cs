using UnityEngine;
using System;

public class TCPServerUI : MonoBehaviour
{
    public int serverPort = 5555;
    [SerializeField] private TCPServer serverReference;

    [SerializeField] private MonoBehaviour hashManagerBehaviour;
    [SerializeField] private MonoBehaviour encryptManagerBehaviour;

    private IHashManager hashManager;
    private IEncryptManager encryptManager;

    private IServer _server;
    void Awake()
    {
        _server = serverReference;

        hashManager = hashManagerBehaviour as IHashManager;
        encryptManager = encryptManagerBehaviour as IEncryptManager;

        if (hashManager == null)
            Debug.LogError("[UI-Server] hashManagerBehaviour does not implement IHashManager. Assign a valid component.");

        if (encryptManager == null)
            Debug.LogError("[UI-Server] encryptManagerBehaviour does not implement IEncryptManager. Assign a valid component.");
    }
    void Start()
    {
        if (_server != null)
        {
            _server.OnMessageReceived += HandleMessageReceived;
            _server.OnConnected += HandleConnection;
            _server.OnDisconnected += HandleDisconnection;
        }
    }
    public void StartServer()
    {
        _server?.StartServer(serverPort);
    }
    public void SendServerMessage()
    {
        if (_server == null || !_server.isServerRunning)
        {
            Debug.Log("[UI-Server] Server is not running");
            return;
        }

        if (hashManager == null)
        {
            Debug.LogWarning("[UI-Server] hashManager is not assigned or does not implement IHashManager");
            return;
        }

        string hashHex = hashManager.GetHash();
        if (string.IsNullOrEmpty(hashHex))
        {
            Debug.Log("[UI-Server] No hash generated to send");
            return;
        }

        if (encryptManager == null)
        {
            Debug.LogWarning("[UI-Server] encryptManager is not assigned or does not implement IEncryptManager");
            return;
        }

        string payload;
        try
        {
            payload = encryptManager.CreateSignedPayload(hashHex);
        }
        catch (Exception ex)
        {
            Debug.Log("[UI-Server] Error creating signed payload: " + ex.Message);
            return;
        }

        _server.SendMessageAsync(payload);
        Debug.Log("[UI-Server] Payload sent: " + payload);
    }

    public void SendSignedManualData()
    {
        if (_server == null || !_server.isServerRunning)
        {
            Debug.Log("[UI-Server] Server is not running");
            return;
        }

        if (hashManager == null || encryptManager == null)
        {
            Debug.LogWarning("[UI-Server] Missing hashManager or encryptManager");
            return;
        }

        string hashHex = hashManager.GetHash();

        if (string.IsNullOrEmpty(hashHex))
        {
            Debug.Log("[UI-Server] No hash generated");
            return;
        }

        try
        {
            string signature = EncryptManual.FirmarHash(hashHex);

            var pk = encryptManager.GetPublicKeyParameters();

            string modulusB64 = Convert.ToBase64String(pk.Modulus);
            string exponentB64 = Convert.ToBase64String(pk.Exponent);

            string payload = $"{hashHex}|{signature}|{modulusB64}|{exponentB64}";

            _server.SendMessageAsync(payload);

            Debug.Log("[UI-Server] Manual payload sent: " + payload);
        }
        catch (Exception ex)
        {
            Debug.Log("[UI-Server] Error sending manual data: " + ex.Message);
        }
    }

    void HandleMessageReceived(string text)
    {
        Debug.Log("[UI-Server] Message received from client: " + text);
    }

    void HandleConnection()
    {
        Debug.Log("[UI-Server] Client connected to server");
    }
    void HandleDisconnection()
    {
        Debug.Log("[UI-Server] Client disconnected from server");
    }
}