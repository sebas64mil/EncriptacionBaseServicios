using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using TMPro;

public class HashManager : MonoBehaviour, IHashManager
{
    [Header("UI")]
    public TMP_InputField inputText;

    private string lastHash;

    public void GenerateHash()
    {
        byte[] data = Encoding.UTF8.GetBytes(inputText.text);

        byte[] hash;
        using (var sha = SHA256.Create())
        {
            hash = sha.ComputeHash(data);
        }

        lastHash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        Debug.Log($"[HashManager] hash = {lastHash}");
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