using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class HashManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputText;
    public TMP_Text resultText;

    private string lastHash;

    public void GenerateHash()
    {
        if (string.IsNullOrEmpty(inputText.text))
        {
            resultText.text = "Texto vacío";
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(inputText.text);

        byte[] hash;
        using (var sha = SHA256.Create())
        {
            hash = sha.ComputeHash(data);
        }

        lastHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        resultText.text = lastHash;
    }

    public void CopyHash()
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