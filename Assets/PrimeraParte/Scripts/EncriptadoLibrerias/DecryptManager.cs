using System;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class DecryptManager : MonoBehaviour, IDecryptManager
{
    [Header("References")]
    public EncryptManager encryptManager;

    private string decryptedResult;

    private string receivedHashHex;
    private string receivedSignatureB64;
    private byte[] receivedModulus;
    private byte[] receivedExponent;

    public void SetReceivedData(string hashHex, string signatureBase64, string modulusBase64, string exponentBase64)
    {
        receivedHashHex = hashHex;
        receivedSignatureB64 = signatureBase64;

        try
        {
            receivedModulus = string.IsNullOrEmpty(modulusBase64) ? null : Convert.FromBase64String(modulusBase64);
            receivedExponent = string.IsNullOrEmpty(exponentBase64) ? null : Convert.FromBase64String(exponentBase64);
            Debug.Log("[DecryptManager] Datos recibidos almacenados");
        }
        catch (Exception ex)
        {
            receivedModulus = null;
            receivedExponent = null;
            Debug.LogError("[DecryptManager] Error al decodificar parámetros públicos: " + ex.Message);
        }
    }

    public void DecryptHash()
    {
        if (string.IsNullOrEmpty(receivedHashHex) || string.IsNullOrEmpty(receivedSignatureB64))
        {
            Debug.Log("[DecryptManager] No hay hash o firma recibida para verificar");
            return;
        }

        try
        {
            byte[] hashBytes = HexStringToByteArray(receivedHashHex);
            byte[] firmaBytes = Convert.FromBase64String(receivedSignatureB64);

            RSAParameters pubParams;

            if (receivedModulus != null && receivedExponent != null)
            {
                pubParams = new RSAParameters { Modulus = receivedModulus, Exponent = receivedExponent };
                Debug.Log("[DecryptManager] Usando llave pública recibida del servidor");
            }
            else if (encryptManager != null)
            {
                pubParams = encryptManager.GetPublicKeyParameters();
                Debug.Log("[DecryptManager] No llegó llave remota; usando llave pública local de EncryptManager");
            }
            else
            {
                Debug.LogWarning("[DecryptManager] No hay llave pública disponible (ni remota ni local)");
                return;
            }

            if (pubParams.Modulus == null || pubParams.Exponent == null)
            {
                Debug.LogWarning("[DecryptManager] Parámetros públicos incompletos");
                return;
            }

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(pubParams);

                bool isValid = rsa.VerifyHash(
                    hashBytes,
                    firmaBytes,
                    HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1
                );

                decryptedResult = isValid ? receivedHashHex.ToLowerInvariant() : null;

                if (isValid)
                    Debug.Log($"[DecryptManager] mensaje verificado: {decryptedResult}");
                else
                    Debug.Log("[DecryptManager] Firma inválida");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[DecryptManager] Error verificando firma: " + ex.Message);
        }
    }

    public void CopyDecrypted()
    {
        if (!string.IsNullOrEmpty(decryptedResult))
        {
            GUIUtility.systemCopyBuffer = decryptedResult;
            Debug.Log("Desencriptado copiado");
        }
    }

    public string GetDecryptedResult()
    {
        return decryptedResult;
    }

    private static byte[] HexStringToByteArray(string hex)
    {
        if (string.IsNullOrEmpty(hex))
            return Array.Empty<byte>();

        hex = hex.Replace(" ", "").Trim();
        if (hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            hex = hex.Substring(2);

        if (hex.Length % 2 != 0)
            throw new FormatException("Longitud de cadena hex inválida.");

        int len = hex.Length / 2;
        byte[] result = new byte[len];
        for (int i = 0; i < len; i++)
        {
            string byteString = hex.Substring(i * 2, 2);
            result[i] = Convert.ToByte(byteString, 16);
        }

        return result;
    }
}