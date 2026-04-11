using System;
using UnityEngine;

public class DecryptManagerManual : MonoBehaviour, IDecryptManager
{
    [Header("References")]
    public EncryptManagerManual encryptManager;

    private string decryptedResult;

    private string receivedHashHex;
    private string receivedSignature; 
    private byte[] receivedModulus;
    private byte[] receivedExponent;

    public void SetReceivedData(string hashHex, string signature, string modulusBase64, string exponentBase64)
    {
        receivedHashHex = hashHex;
        receivedSignature = signature;

        try
        {
            receivedModulus = string.IsNullOrEmpty(modulusBase64) ? null : Convert.FromBase64String(modulusBase64);
            receivedExponent = string.IsNullOrEmpty(exponentBase64) ? null : Convert.FromBase64String(exponentBase64);

            Debug.Log("[DecryptManagerManual] Datos recibidos almacenados");
            Debug.Log($"[Decrypt] Hash recibido: {receivedHashHex}");
            Debug.Log($"[Decrypt] Firma recibida: {receivedSignature}");
        }
        catch (Exception ex)
        {
            receivedModulus = null;
            receivedExponent = null;
            Debug.LogError("[DecryptManagerManual] Error al decodificar parámetros públicos: " + ex.Message);
        }
    }

    public void DecryptHash()
    {
        if (string.IsNullOrEmpty(receivedHashHex) || string.IsNullOrEmpty(receivedSignature))
        {
            Debug.Log("[DecryptManagerManual] No hay hash o firma recibida para verificar");
            return;
        }

        try
        {
            byte[] modulusToUse = null;
            byte[] exponentToUse = null;

            if (receivedModulus != null && receivedExponent != null)
            {
                modulusToUse = receivedModulus;
                exponentToUse = receivedExponent;
                Debug.Log("[DecryptManagerManual] Usando llave pública recibida");
            }
            else if (encryptManager != null)
            {
                var pk = encryptManager.GetPublicKeyParameters();
                modulusToUse = pk.Modulus;
                exponentToUse = pk.Exponent;
                Debug.Log("[DecryptManagerManual] Usando llave pública local");
            }
            else
            {
                Debug.LogWarning("[DecryptManagerManual] No hay llave pública disponible");
            }

            bool isValid = DecryptManual.VerificarFirma(
                receivedHashHex,
                receivedSignature,
                modulusToUse,
                exponentToUse
            );

            decryptedResult = isValid ? receivedHashHex : null;

            if (isValid)
                Debug.Log($"[DecryptManagerManual]  Mensaje verificado: {decryptedResult}");
            else
                Debug.Log("[DecryptManagerManual]  Firma inválida");
        }
        catch (Exception ex)
        {
            Debug.LogError("[DecryptManagerManual] Error verificando firma: " + ex.Message);
        }
    }

    public void CopyDecrypted()
    {
        if (!string.IsNullOrEmpty(decryptedResult))
        {
            GUIUtility.systemCopyBuffer = decryptedResult;
            Debug.Log("Desencriptado copiado");
        }
        else
        {
            Debug.LogWarning("No hay resultado para copiar");
        }
    }

    public string GetDecryptedResult()
    {
        return decryptedResult;
    }
}