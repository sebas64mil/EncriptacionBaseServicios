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

        // Cast behaviours to interfaces
        hashManager = hashManagerBehaviour as IHashManager;
        encryptManager = encryptManagerBehaviour as IEncryptManager;

        if (hashManager == null)
            Debug.LogError("[UI-Server] hashManagerBehaviour no implementa IHashManager (asigna un componente válido)");

        if (encryptManager == null)
            Debug.LogError("[UI-Server] encryptManagerBehaviour no implementa IEncryptManager (asigna un componente válido)");
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
            Debug.Log("[UI-Server] El servidor no está en ejecución");
            return;
        }

        if (hashManager == null)
        {
            Debug.LogWarning("[UI-Server] hashManager no asignado o no implementa IHashManager");
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
            Debug.LogWarning("[UI-Server] encryptManager no asignado o no implementa IEncryptManager");
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

    public void SendSignedManualData()
    {
        if (_server == null || !_server.isServerRunning)
        {
            Debug.Log("[UI-Server] El servidor no está en ejecución");
            return;
        }

        if (hashManager == null || encryptManager == null)
        {
            Debug.LogWarning("[UI-Server] Falta hashManager o encryptManager");
            return;
        }

        string hashHex = hashManager.GetHash();

        if (string.IsNullOrEmpty(hashHex))
        {
            Debug.Log("[UI-Server] No hay hash generado");
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

            Debug.Log("[UI-Server] Payload MANUAL enviado: " + payload);
        }
        catch (Exception ex)
        {
            Debug.Log("[UI-Server] Error enviando datos manuales: " + ex.Message);
        }
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