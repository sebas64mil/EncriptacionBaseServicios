using System;
using System.Numerics;
using System.Globalization;
using UnityEngine;

public class EncryptManual
{
    private static BigInteger n = new BigInteger(3233);
    private static BigInteger d = new BigInteger(2753);
    private static BigInteger e = new BigInteger(17);

    private static BigInteger HashHexToNumber(string hashHex)
    {
        if (string.IsNullOrEmpty(hashHex))
            throw new ArgumentException("hashHex inválido");

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

    public static string FirmarHash(string hashHex)
    {
        BigInteger mFull = HashHexToNumber(hashHex);
        BigInteger m = mFull % n; 

        BigInteger firma = BigInteger.ModPow(m, d, n);

        Debug.Log($"[EncryptManual] mFull={mFull} m(mod n)={m} firma={firma} n={n} d={d}");

        return firma.ToString();
    }

    public static bool VerificarFirma(string hashHex, string firmaStr)
    {
        if (string.IsNullOrEmpty(firmaStr))
            return false;

        BigInteger firma = BigInteger.Parse(firmaStr);
        BigInteger esperadoFull = HashHexToNumber(hashHex);
        BigInteger esperado = esperadoFull % n;

        BigInteger resultado = BigInteger.ModPow(firma, e, n);

        Debug.Log($"[EncryptManual] verificar: firma={firma} resultado={resultado} esperadoFull={esperadoFull} esperado(mod n)={esperado} n={n} e={e}");

        return resultado == esperado;
    }
}