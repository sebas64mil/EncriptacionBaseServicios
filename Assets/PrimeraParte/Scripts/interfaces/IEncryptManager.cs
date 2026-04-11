using System.Security.Cryptography;

public interface IEncryptManager
{
    void EncryptHash();
    void CopyEncrypted();

    RSAParameters GetPublicKeyParameters();
    string CreateSignedPayload(string hashHex);
}