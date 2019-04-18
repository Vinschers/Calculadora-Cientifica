using System;
using System.IO;
using System.Text.RegularExpressions;

public class Calculadora
{
    private class Conta : IComparable<Conta>
    {
        public string Infixa { get; set; }
        public string Posfixa { get; set; }
        public double Resultado { get; set; }
        public int CompareTo(Conta outra)
        {
            return Infixa.CompareTo(outra.Infixa);
        }
        public Conta()
        {
            Infixa = Posfixa = "";
            Resultado = 0;
        }
    }
    private bool[,] precedencia;
    Pilha<Conta> contas;

    public string Infixa
    {
        get
        {
            return contas.EstaVazia()?"":contas.Topo.Infixa;
        }
        set => contas.Topo.Infixa = value;
    }
    public string Posfixa { get => contas.Topo.Posfixa; }

    public Calculadora()
    {
        contas = new Pilha<Conta>();
        contas.Empilhar(new Conta());
        precedencia = new bool[7, 7];
        StreamReader leitor = new StreamReader("precedencia.txt");
        int indiceLinha = 0;
        while (!leitor.EndOfStream)
        {
            string linha = leitor.ReadLine();
            string[] valores = linha.Split(' ');
            for (int indiceColuna = 0; indiceColuna < valores.Length; indiceColuna++)
                precedencia[indiceLinha, indiceColuna] = valores[indiceColuna].Equals("1") ? true : false;
            indiceLinha++;
        }
        leitor.Close();
    }
    public double CalcularExpressao()
    {
        Fila<string> posfixa = CalcularPosfixa();
        Pilha<double> resultados = new Pilha<double>();
        double result = 0;
        while (!posfixa.EstaVazia())
        {
            string atual = posfixa.Retirar();
            if (IsNumeric(atual))
                resultados.Empilhar(int.Parse(atual));
            else
            {
                double segundo = resultados.Pop();
                double primeiro = resultados.Pop();
                resultados.Empilhar(RealizarOperacao(primeiro, segundo, atual));
            }
        }
        result = resultados.Pop();
        contas.Topo.Resultado = result;
        contas.Empilhar(new Conta());
        return result;
    }
    private Fila<string> CalcularPosfixa()
    {
        string infixa = contas.Topo.Infixa;
        Regex.Replace(infixa, @"\s+", "");
        Pilha<char> pilha = new Pilha<char>();
        Fila<string> pos = new Fila<string>();
        char atual;
        while (infixa.Length > 0)
        {
            atual = infixa[0];
            int i = 1;
            if ((atual < 48 || atual > 57) && atual != 32) //nao eh numero
            {
                while (!pilha.EstaVazia() && HaPrecedencia(pilha.Topo, atual))
                    pos.Enfileirar(pilha.Pop().ToString());
                pilha.Empilhar(atual);
            }
            else if (atual != 32) //eh numero
            {
                string numero = atual + "";
                try
                {
                    while (IsNumeric(infixa.Substring(0, i) + "") || infixa.Substring(i).Equals(","))
                        numero = infixa.Substring(0, i++);
                }
                catch
                {
                    char c = infixa[infixa.Length - 1];
                    if (c < 48 && c > 57)
                        numero += c;
                }
                i--;
                pos.Enfileirar(numero);
            }
            infixa = infixa.Substring(i);
        }
        while (!pilha.EstaVazia())
            pos.Enfileirar(pilha.Pop() + "");
        string[] posString = pos.ToArray();
        string posfixa = "";
        atual = 'A';
        int repeticoes = 0;
        for (int i = 0; i < posString.Length; i++)
        {
            if (IsNumeric(posString[i]))
            {
                posfixa += Convert.ToChar(atual++) + "";
                for (int rep = 0; rep < repeticoes; rep++)
                    posfixa += "'";
                if (atual == 91) //acabou o alfabeto
                {
                    repeticoes++;
                    atual = 'A';
                }
            }
            else
                posfixa += posString[i];
        }
        contas.Topo.Posfixa = posfixa;
        return pos;
    }
    private bool HaPrecedencia(char i, char j)
    {
        if (i == '-')
            i = Convert.ToChar(i - 1);
        else if (i == '/')
            i = Convert.ToChar(i - 2);
        else if (i == '^')
            i = Convert.ToChar(i - 48);

        if (j == '-')
            j = Convert.ToChar(j - 1);
        else if (j == '/')
            j = Convert.ToChar(j - 2);
        else if (j == '^')
            j = Convert.ToChar(j - 48);

        i = Convert.ToChar(i - 40);
        j = Convert.ToChar(j - 40);

        return precedencia[i, j];
    }
    private bool IsNumeric(string str)
    {
        for (int i = 0; i < str.Length; i++)
            if (str[i] < 48 || str[i] > 57)
                return false;
        return true;
    }
    private double RealizarOperacao(double a, double b, string o)
    {
        switch (o)
        {
            case "+":
                return a + b;
            case "-":
                return a - b;
            case "*":
                return a * b;
            case "/":
                return a / b;
            case "^":
                return Math.Pow(a, b);
            default:
                return 0;
        }
    }
}