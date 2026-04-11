using System.Collections.Generic;
using System.Text;

public static class HashManual
{
    public static string GenerarHash(string input)
    {
        var paso1 = Expandir(input);
        var paso2 = ConvertirANumeros(paso1);
        var paso3 = AplicarFuncionMatematica(paso2);

        var ronda = paso3;
        for (int i = 0; i < 3; i++)
        {
            ronda = Mezclar(ronda);
            ronda = AplicarXOR(ronda);
        }

        var pasoFinal = AjustarATamanoFijo(ronda);
        var hash = ConvertirAHex(pasoFinal);

        return hash;
    }

    private static List<char> Expandir(string input)
    {
        List<char> resultado = new List<char>();

        foreach (char c in input)
        {
            int valor = (int)c;

            char siguiente1 = (char)((valor + 2) % 256);
            char siguiente2 = (char)((valor + 1) % 256);

            resultado.Add(siguiente1);
            resultado.Add(siguiente2);
        }

        return resultado;
    }

    private static List<int> ConvertirANumeros(List<char> chars)
    {
        List<int> numeros = new List<int>();

        foreach (char c in chars)
        {
            numeros.Add((int)c);
        }

        return numeros;
    }

    private static List<int> AplicarFuncionMatematica(List<int> numeros)
    {
        for (int i = 0; i < numeros.Count; i++)
        {
            numeros[i] = (numeros[i] * 7 + 13) % 256;
        }

        return numeros;
    }

    private static List<int> Mezclar(List<int> lista)
    {
        int n = lista.Count;
        List<int> mezclado = new List<int>(new int[n]);

        for (int i = 0; i < n; i++)
        {
            int nuevaPos = (i * 7) % n;
            mezclado[nuevaPos] = lista[i];
        }

        return mezclado;
    }

    private static List<int> AplicarXOR(List<int> lista)
    {
        List<int> resultado = new List<int>();

        for (int i = 0; i < lista.Count - 1; i++)
        {
            int a = lista[i];
            int b = lista[i + 1];

            int xor = a ^ b;

            if (i % 2 == 0)
            {
                if (xor == 0)
                    resultado.Add((a / 2 + 7) % 256);
                else
                    resultado.Add(a);
            }
            else
            {
                if (xor == 0)
                    resultado.Add((a * 2 + 3) % 256);
                else
                    resultado.Add(a);
            }
        }

        return resultado;
    }

    private static List<int> AjustarATamanoFijo(List<int> lista)
    {
        List<int> resultado = new List<int>(lista);

        while (resultado.Count > 32)
        {
            List<int> temp = new List<int>();

            for (int i = 0; i < resultado.Count - 1; i += 2)
            {
                int combinado = (resultado[i] ^ resultado[i + 1]) % 256;
                temp.Add(combinado);
            }

            if (resultado.Count % 2 != 0)
                temp.Add(resultado[resultado.Count - 1]);

            resultado = temp;
        }

        while (resultado.Count < 32)
        {
            int ultimo = resultado[resultado.Count - 1];
            int nuevo = (ultimo * 31 + 17) % 256;
            resultado.Add(nuevo);
        }

        return resultado;
    }

    private static string ConvertirAHex(List<int> lista)
    {
        StringBuilder hash = new StringBuilder();

        foreach (int num in lista)
        {
            hash.Append(num.ToString("X2"));
        }

        return hash.ToString();
    }
}
