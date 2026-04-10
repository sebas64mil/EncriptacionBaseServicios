using UnityEngine;
using TMPro;

public class CompareManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField hashInput;        // Original hash to compare _/\_ Hash original a comparar
    public TMP_InputField decryptedInput;   // Decrypted hash to compare _/\_ Hash desencriptado a comparar
    public TMP_Text resultText;

    [Header("References")]
    public HashManager    hashManager;      // To auto-fill hash field _/\_ Para autocompletar el campo hash
    public DecryptManager decryptManager;   // To auto-fill decrypted field _/\_ Para autocompletar el campo desencriptado

    public void Compare() // Compares both hashes and shows result _/\_ Compara ambos hashes y muestra el resultado
    {
        // Auto-fill from managers if inputs are empty _/\_ Rellena automáticamente si los campos están vacíos
        if (string.IsNullOrEmpty(hashInput.text) && hashManager != null)
            hashInput.text = hashManager.GetHash();

        if (string.IsNullOrEmpty(decryptedInput.text) && decryptManager != null)
            decryptedInput.text = decryptManager.GetDecryptedResult();

        string hash1 = hashInput.text.Trim().ToLowerInvariant();
        string hash2 = decryptedInput.text.Trim().ToLowerInvariant();

        if (string.IsNullOrEmpty(hash1) || string.IsNullOrEmpty(hash2))
        {
            resultText.text  = "Campos vacíos";
            resultText.color = Color.yellow;
            return;
        }

        bool match = hash1 == hash2;

        resultText.text  = match ? "✅ Ta bien" : "❌ No coinciden";
        resultText.color = match ? Color.green  : Color.red;

        Debug.Log(match ? "Hashes coinciden" : "Hashes diferentes");
    }
}