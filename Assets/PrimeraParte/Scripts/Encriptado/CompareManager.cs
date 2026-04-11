using UnityEngine;
using TMPro;

public class CompareManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField hashInput;        // Hash original a comparar (asignar en el Inspector)
    public TMP_InputField decryptedInput;   // Hash desencriptado a comparar (asignar en el Inspector)

    public void Compare() // Compara ambos hashes y muestra el resultado por consola
    {
        if (hashInput == null || decryptedInput == null)
        {
            Debug.LogWarning("[CompareManager] Los campos InputField no están asignados en el Inspector");
            return;
        }

        string hash1 = hashInput.text?.Trim().ToLowerInvariant() ?? string.Empty;
        string hash2 = decryptedInput.text?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrEmpty(hash1) || string.IsNullOrEmpty(hash2))
        {
            Debug.Log("[CompareManager] Campos vacíos");
            return;
        }

        bool match = hash1 == hash2;
        Debug.Log(match ? "[CompareManager] ✅ Hashes coinciden" : "[CompareManager] ❌ Hashes diferentes");
    }
}