using UnityEngine;
using TMPro;

public class HashManaherManual : MonoBehaviour, IHashManager
{
    [Header("UI")]
    public TMP_InputField inputText;

    private string lastHash;

    public void GenerateHash()
    {
        string input = inputText != null ? inputText.text : string.Empty;

        lastHash = HashManual.GenerarHash(input);
        Debug.Log($"[HashManaherManual] hash = {lastHash}");
    }

    public void CopyHash()
    {
        if (!string.IsNullOrEmpty(lastHash))
        {
            GUIUtility.systemCopyBuffer = lastHash;
            Debug.Log("Hash copied");
        }
    }

    public string GetHash()
    {
        return lastHash;
    }
}