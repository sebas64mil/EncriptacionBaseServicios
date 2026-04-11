using UnityEngine;
using TMPro;

public class CompareManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField hashInput;
    public TMP_InputField decryptedInput;

    public void Compare()
    {
        if (hashInput == null || decryptedInput == null)
        {
            Debug.LogWarning("[CompareManager] Los campos InputField no están asignados");
            return;
        }

        string hash1 = hashInput.text ?? string.Empty;
        string hash2 = decryptedInput.text ?? string.Empty;

        if (string.IsNullOrEmpty(hash1) || string.IsNullOrEmpty(hash2))
        {
            Debug.Log("[CompareManager] Campos vacíos");
            return;
        }

        bool match = string.Equals(hash1, hash2, System.StringComparison.Ordinal);

        Debug.Log(match
            ? "[CompareManager]  Hashes coinciden EXACTAMENTE"
            : "[CompareManager]  Hashes NO coinciden");
    }
}