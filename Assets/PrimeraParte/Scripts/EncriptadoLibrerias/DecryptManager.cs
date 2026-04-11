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
            Debug.Log("[DecryptManager] Received data stored");
        }
        catch (Exception ex)
        {
            receivedModulus = null;
            receivedExponent = null;
            Debug.LogError("[DecryptManager] Error decoding public parameters: " + ex.Message);
        }
    }

    public void DecryptHash()
    {
        if (string.IsNullOrEmpty(receivedHashHex) || string.IsNullOrEmpty(receivedSignatureB64))
        {
            Debug.Log("[DecryptManager] No hash or signature received to verify");
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
                Debug.Log("[DecryptManager] Using public key received from server");
            }
            else if (encryptManager != null)
            {
                pubParams = encryptManager.GetPublicKeyParameters();
                Debug.Log("[DecryptManager] Remote key not received; using local EncryptManager public key");
            }
            else
            {
                Debug.LogWarning("[DecryptManager] No public key available (neither remote nor local)");
                return;
            }

            if (pubParams.Modulus == null || pubParams.Exponent == null)
            {
                Debug.LogWarning("[DecryptManager] Public parameters incomplete");
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
                    Debug.Log($"[DecryptManager] message verified: {decryptedResult}");
                else
                    Debug.Log("[DecryptManager] Invalid signature");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[DecryptManager] Error verifying signature: " + ex.Message);
        }
    }

    public void CopyDecrypted()
    {
        if (!string.IsNullOrEmpty(decryptedResult))
        {
            GUIUtility.systemCopyBuffer = decryptedResult;
            Debug.Log("Decrypted copied");
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
            throw new FormatException("Invalid hex string length.");

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