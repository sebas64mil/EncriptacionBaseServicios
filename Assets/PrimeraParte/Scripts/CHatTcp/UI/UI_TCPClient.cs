using System;
using TMPro;
using UnityEngine;

public class UI_TCPClient : MonoBehaviour
{
    public int serverPort = 5555;
    public string serverAddress = "127.0.0.1";
    [SerializeField] private TCPClient clientReference;
    
    [SerializeField] private MonoBehaviour decryptManagerBehaviour;

    private IDecryptManager decryptManager;

    private IClient _client;
    private string lastReceivedMessage;

    void Awake()
    {
        _client = clientReference;

        decryptManager = decryptManagerBehaviour as IDecryptManager;
        if (decryptManager == null)
            Debug.LogWarning("[UI-Client] decryptManagerBehaviour no implementa IDecryptManager (asigna un componente válido)");
    }
    void Start()
    {
        if (_client != null)
        {
            _client.OnMessageReceived += HandleMessageReceived;
            _client.OnConnected += HandleConnection;
            _client.OnDisconnected += HandleDisconnection;
        }
    }

    public void ConnectClient()
    {
        _client?.ConnectToServer(serverAddress, serverPort);
    }

 
    public void CopyLastServerMessage()
    {
        if (string.IsNullOrEmpty(lastReceivedMessage))
        {
            Debug.Log("No hay mensaje recibido para copiar");
            return;
        }

        GUIUtility.systemCopyBuffer = lastReceivedMessage;
        Debug.Log("[UI-Client] Mensaje copiado al portapapeles: " + lastReceivedMessage);
    }

    void HandleMessageReceived(string text)
    {
        lastReceivedMessage = text;
        Debug.Log("[UI-Client] Message received from server: " + text);

        try
        {
            string[] parts = text.Split('|');
            if (parts.Length == 4)
            {
                string hashHex = parts[0];
                string signatureB64 = parts[1];
                string modulusB64 = parts[2];
                string exponentB64 = parts[3];

                if (decryptManager != null)
                {
                    decryptManager.SetReceivedData(hashHex, signatureB64, modulusB64, exponentB64);
                    Debug.Log("[UI-Client] Datos pasados a DecryptManager (listos para desencriptar)");
                }
                else
                {
                    Debug.LogWarning("[UI-Client] decryptManager no asignado en el inspector o no implementa IDecryptManager");
                }
            }
            else
            {
                Debug.Log("[UI-Client] Payload no tiene el formato esperado, mensaje crudo: " + text);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("[UI-Client] Error parseando mensaje del servidor: " + ex.Message);
        }
    }

    void HandleConnection()
    {
        Debug.Log("[UI-Client] Client Connected to Server");
    }
    void HandleDisconnection()
    {
        Debug.Log("[UI-Client] Client Disconnect from Server");
    }
}