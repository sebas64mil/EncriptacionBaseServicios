using System;
using System.Numerics;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class EncryptManagerManual : MonoBehaviour, IEncryptManager
{
    [Header("UI")]
    public TMP_InputField hashInput;

    private string encryptedText;

    private static readonly BigInteger n = new BigInteger(3233);
    private static readonly BigInteger e = new BigInteger(17);

    public void EncryptHash()
    {
        try
        {
            string hashHex = hashInput != null ? hashInput.text : string.Empty;

            if (string.IsNullOrEmpty(hashHex))
            {
                Debug.LogWarning("Hash vacío");
                return;
            }

            string signature = EncryptManual.FirmarHash(hashHex);

            encryptedText = signature;

            Debug.Log($"[EncryptManagerManual] Firma generada = {encryptedText}");
        }
        catch (Exception ex)
        {
            Debug.LogError("[EncryptManagerManual] Error firmando: " + ex.Message);
        }
    }

    public void CopyEncrypted()
    {
        if (!string.IsNullOrEmpty(encryptedText))
        {
            GUIUtility.systemCopyBuffer = encryptedText;
            Debug.Log("Firma copiada");
        }
        else
        {
            Debug.LogWarning("No hay firma para copiar");
        }
    }

    public string CreateSignedPayload(string hashHex)
    {
        if (string.IsNullOrEmpty(hashHex))
            throw new ArgumentException("hashHex vacío", nameof(hashHex));

        string signature = EncryptManual.FirmarHash(hashHex);

        return $"{hashHex}|{signature}";
    }

    public RSAParameters GetPublicKeyParameters()
    {
        return new RSAParameters
        {
            Modulus = BigIntegerToBytes(n),
            Exponent = BigIntegerToBytes(e)
        };
    }

    private static byte[] BigIntegerToBytes(BigInteger value)
    {
        byte[] bytes = value.ToByteArray();

        Array.Reverse(bytes);

        int start = 0;
        while (start < bytes.Length && bytes[start] == 0x00)
            start++;

        byte[] result = new byte[bytes.Length - start];
        Array.Copy(bytes, start, result, 0, result.Length);

        return result;
    }

    public string GetEncrypted()
    {
        return encryptedText;
    }
}