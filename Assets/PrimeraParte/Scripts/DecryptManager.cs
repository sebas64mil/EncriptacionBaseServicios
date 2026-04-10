using System;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class DecryptManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField hashInput;       // Hash original (hex)
    public TMP_InputField signatureInput;  // Firma en Base64
    public TMP_Text resultText;

    [Header("References")]
    public EncryptManager encryptManager;  // Reference to get the RSA instance _/\_ Referencia para obtener la instancia RSA

    private string decryptedResult;

    public void DecryptHash() // Verifies the signature using the public key _/\_ Verifica la firma usando la llave pública
    {
        if (string.IsNullOrEmpty(hashInput.text) || string.IsNullOrEmpty(signatureInput.text))
        {
            resultText.text = "Campos vacíos";
            return;
        }

        try
        {
            // Get hash bytes from hex input _/\_ Obtiene los bytes del hash desde el input hex
            byte[] hashBytes = HexStringToByteArray(hashInput.text);

            // Get signature bytes from Base64 input _/\_ Obtiene los bytes de la firma desde el input Base64
            byte[] firmaBytes = Convert.FromBase64String(signatureInput.text);

            // Get the RSA instance from EncryptManager _/\_ Obtiene la instancia RSA desde EncryptManager
            RSA rsa = encryptManager.GetRSA();

            bool isValid = rsa.VerifyHash(
                hashBytes,
                firmaBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            // Store the original hash hex as the "decrypted" result for comparison _/\_ Guarda el hash original para la comparación
            decryptedResult = isValid ? hashInput.text.ToLowerInvariant() : null;

            resultText.text = isValid
                ? hashInput.text.ToLowerInvariant()
                : "Firma inválida";
        }
        catch
        {
            resultText.text = "Datos inválidos";
        }
    }

    public void CopyDecrypted() // Copy the decrypted result to the clipboard _/\_ Copia el resultado al portapapeles
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

    private static byte[] HexStringToByteArray(string hex) // Converts hex string to byte array _/\_ Convierte cadena hex a arreglo de bytes
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