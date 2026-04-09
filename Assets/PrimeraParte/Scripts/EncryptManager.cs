using System;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class EncryptManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField hashInput;
    public TMP_Text resultText;

    private RSA rsa;
    private string encryptedText;

    private void Awake() // An instance is created and the key size is determined _/\_ Se crea una instancia y determina el tamaño de la clave 
    {
        rsa = RSA.Create();
        rsa.KeySize = 2048; 
    }

    public void EncryptHash()
    {
        if (string.IsNullOrEmpty(hashInput.text)) //verify that the text field is not empty _/\_ verifica que el campo de texto no esté vacío
        {
            resultText.text = "Hash vacío";
            return;
        }

        try // Convert the hash to bytes, sign it with the private key, and convert it to text _/\_ Convierte el hash a bytes, firma con la clave privada y lo convierte a texto
        {
            byte[] hashBytes = HexStringToByteArray(hashInput.text);

            byte[] firma = rsa.SignHash(
                hashBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            encryptedText = Convert.ToBase64String(firma);
            resultText.text = encryptedText;
        }
        catch
        {
            resultText.text = "Hash inválido";
        }
    }

    public void CopyEncrypted() // Copy the encrypted text to the clipboard _/\_ Copia el texto encriptado al portapapeles
    {
        if (!string.IsNullOrEmpty(encryptedText))
        {
            GUIUtility.systemCopyBuffer = encryptedText;
            Debug.Log("Encriptado copiado");
        }
    }

    public RSA GetRSA() 
    {
        return rsa;
    }

    private static byte[] HexStringToByteArray(string hex) // class used to convert to a byte array _/\_ clase que se usa para convertir a un arreglo de bytes
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