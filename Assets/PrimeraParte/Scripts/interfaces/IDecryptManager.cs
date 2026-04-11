public interface IDecryptManager
{
    void SetReceivedData(string hashHex, string signatureBase64, string modulusBase64, string exponentBase64);
    void DecryptHash();
    void CopyDecrypted();
    string GetDecryptedResult();
}