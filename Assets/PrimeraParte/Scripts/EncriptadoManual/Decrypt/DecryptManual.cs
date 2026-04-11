using System;
using System.Numerics;
using System.Globalization;
using UnityEngine;

public static class DecryptManual
{
    private static readonly BigInteger DefaultN = new BigInteger(3233);
    private static readonly BigInteger DefaultE = new BigInteger(17);

    private static BigInteger HashHexToNumber(string hashHex)
    {
        if (string.IsNullOrEmpty(hashHex))
            return BigInteger.Zero;

        string hex = hashHex.Trim();
        if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            hex = hex.Substring(2);

        try
        {
            return BigInteger.Parse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
        }
        catch
        {
            BigInteger result = BigInteger.Zero;
            foreach (char c in hex)
            {
                int v;
                if (c >= '0' && c <= '9') v = c - '0';
                else if (c >= 'A' && c <= 'F') v = c - 'A' + 10;
                else if (c >= 'a' && c <= 'f') v = c - 'a' + 10;
                else throw new ArgumentException("hashHex contiene caracteres no hexadecimales");

                result = (result << 4) | v;
            }
            return result;
        }
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

            BigInteger esperadoFull = HashHexToNumber(hashHex);
            BigInteger esperado = esperadoFull % n;

            BigInteger resultado = BigInteger.ModPow(firma, e, n);

            Debug.Log($"[DecryptManual] firma={firma} resultado={resultado} esperadoFull={esperadoFull} esperado(mod n)={esperado} n={n} e={e}");

            return resultado == esperado;
        }
        catch (Exception ex)
        {
            Debug.LogError("[DecryptManual] Error verificando firma: " + ex.Message);
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