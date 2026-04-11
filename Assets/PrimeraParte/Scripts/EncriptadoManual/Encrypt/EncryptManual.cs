using System;
using System.Numerics;

public class EncryptManual
{
    private static BigInteger n = new BigInteger(3233);
    private static BigInteger d = new BigInteger(2753);
    private static BigInteger e = new BigInteger(17);

    private static BigInteger HashHexToNumber(string hashHex)
    {
        if (string.IsNullOrEmpty(hashHex))
            throw new ArgumentException("hashHex inválido");

        BigInteger result = BigInteger.Zero;

        foreach (char c in hashHex)
        {
            int value = Convert.ToInt32(c);
            result = (result * 31 + value) % n; 
        }

        return result;
    }

    public static string FirmarHash(string hashHex)
    {
        BigInteger m = HashHexToNumber(hashHex);

        BigInteger firma = BigInteger.ModPow(m, d, n);

        return firma.ToString();
    }

    public static bool VerificarFirma(string hashHex, string firmaStr)
    {
        if (string.IsNullOrEmpty(firmaStr))
            return false;

        BigInteger firma = BigInteger.Parse(firmaStr);
        BigInteger esperado = HashHexToNumber(hashHex);

        BigInteger resultado = BigInteger.ModPow(firma, e, n);

        return resultado == esperado;
    }
}