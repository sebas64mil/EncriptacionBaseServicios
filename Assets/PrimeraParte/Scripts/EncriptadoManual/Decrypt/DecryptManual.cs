using System;
using System.Numerics;

public static class DecryptManual
{
    private static readonly BigInteger DefaultN = new BigInteger(3233);
    private static readonly BigInteger DefaultE = new BigInteger(17);

    private static BigInteger HashHexToNumber(string hashHex, BigInteger n)
    {
        BigInteger result = BigInteger.Zero;

        foreach (char c in hashHex)
        {
            int value = Convert.ToInt32(c);
            result = (result * 31 + value) % n;
        }

        return result;
    }

    public static bool VerificarFirma(string hashHex, string firmaStr, byte[] modulus = null, byte[] exponent = null)
    {
        if (string.IsNullOrEmpty(hashHex) || string.IsNullOrEmpty(firmaStr))
            return false;

        try
        {
            BigInteger n = modulus != null ? BigIntegerFromBytes(modulus) : DefaultN;
            BigInteger e = exponent != null ? BigIntegerFromBytes(exponent) : DefaultE;

            BigInteger firma = BigInteger.Parse(firmaStr);

            BigInteger resultado = BigInteger.ModPow(firma, e, n);

            BigInteger esperado = HashHexToNumber(hashHex, n);

            return resultado == esperado;
        }
        catch
        {
            return false;
        }
    }

    private static BigInteger BigIntegerFromBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return BigInteger.Zero;

        byte[] temp = new byte[bytes.Length + 1];

        for (int i = 0; i < bytes.Length; i++)
            temp[i] = bytes[bytes.Length - 1 - i];

        temp[temp.Length - 1] = 0x00;

        return new BigInteger(temp);
    }
}