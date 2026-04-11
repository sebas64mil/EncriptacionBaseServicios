using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class HashManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputText;

    private string lastHash;

    public void GenerateHash() // It generates a SHA256 hash from the text and displays it _/\_ Genera un hash SHA256 a partir del texto y lo muestra
    {


        byte[] data = Encoding.UTF8.GetBytes(inputText.text); // Convert the text to bytes _/\_ Convierte el texto a bytes

        byte[] hash;
        using (var sha = SHA256.Create())
        {
            hash = sha.ComputeHash(data);
        }

        lastHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // Convert the hash to a hexadecimal string _/\_ Convierte el hash a hexadecimal
        Debug.Log($"[HashManager] hash = {lastHash}");
    }

    public void CopyHash() // Copy the last generated hash to the clipboard _/\_ Copia el último hash generado al portapapeles
    {
        if (!string.IsNullOrEmpty(lastHash))
        {
            GUIUtility.systemCopyBuffer = lastHash;
            Debug.Log("Hash copiado");
        }
    }

    public string GetHash()
    {
        return lastHash;
    }
}