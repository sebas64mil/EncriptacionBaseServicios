using System;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class EncryptManager : MonoBehaviour, IEncryptManager
{
    [Header("UI")]
    public TMP_InputField hashInput;

    private RSA rsa;
    private string encryptedText;

    private void Awake()
    {
        rsa = RSA.Create();
        rsa.KeySize = 2048;
    }

    public void EncryptHash()
    {
        try
        {
            byte[] hashBytes = HexStringToByteArray(hashInput.text);

            byte[] firma = rsa.SignHash(
                hashBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            encryptedText = Convert.ToBase64String(firma);
            Debug.Log($"[EncryptManager] firma = {encryptedText}");
        }
        catch (Exception ex)
        {
            Debug.LogError("[EncryptManager] Error firmando: " + ex.Message);
        }
    }

    public void CopyEncrypted()
    {
        if (!string.IsNullOrEmpty(encryptedText))
        {
            GUIUtility.systemCopyBuffer = encryptedText;
            Debug.Log("Encriptado copiado");
        }
    }

    public RSAParameters GetPublicKeyParameters()
    {
        return rsa.ExportParameters(false);
    }

    public string CreateSignedPayload(string hashHex)
    {
        if (string.IsNullOrEmpty(hashHex))
            throw new ArgumentException("hashHex vacío", nameof(hashHex));

        byte[] hashBytes = HexStringToByteArray(hashHex);
        byte[] signature = rsa.SignHash(hashBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        string signatureB64 = Convert.ToBase64String(signature);

        RSAParameters pub = rsa.ExportParameters(false);
        string modulusB64 = pub.Modulus != null ? Convert.ToBase64String(pub.Modulus) : "";
        string exponentB64 = pub.Exponent != null ? Convert.ToBase64String(pub.Exponent) : "";

        return $"{hashHex}|{signatureB64}|{modulusB64}|{exponentB64}";
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