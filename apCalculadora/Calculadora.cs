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
            return contas.EstaVazia() ? "" : contas.Topo.Infixa;
        }
        set => contas.Topo.Infixa = value;
    }
    public string Posfixa { get => contas.Topo.Posfixa; }

    public Calculadora()
    {
        contas = new Pilha<Conta>();
        contas.Empilhar(new Conta());
        precedencia = new bool[7, 7];
        StreamReader leitor = new StreamReader("../../../precedencia.txt");
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
                resultados.Empilhar(double.Parse(atual));
            else
            {
                double segundo = resultados.Pop();
                double primeiro = resultados.Pop();
                double resultadoAtual = RealizarOperacao(primeiro, segundo, atual);
                resultados.Empilhar(resultadoAtual);
            }
        }
        result = resultados.Pop();
        contas.Topo.Resultado = result;
        contas.Empilhar(new Conta());
        return result;
    }
    private Fila<string> CalcularPosfixa()
    {
        string[] infixa = contas.Topo.Infixa.Split(' ');
        Pilha<char> pilha = new Pilha<char>();
        Fila<string> pos = new Fila<string>();
        for (int i = 0; i < infixa.Length; i++)
        {
            string atual = infixa[i];
            if (IsNumeric(atual))
            {
                pos.Enfileirar(atual);
            }
            else
            {
                while (!pilha.EstaVazia() && HaPrecedencia(pilha.Topo, atual[0]))
                {
                    string operandoAtual = pilha.Pop().ToString();
                    if (!operandoAtual.Equals("(") && !operandoAtual.Equals(")"))
                        pos.Enfileirar(operandoAtual);
                }
                if (!pilha.EstaVazia() && pilha.Topo == '(' && atual[0] == ')')
                    pilha.Pop();
                if (atual[0] != ')')
                    pilha.Empilhar(atual[0]);
            }
        }
        while (!pilha.EstaVazia())
        {
            char aux = pilha.Pop();
            if (aux != '(' && aux != ')')
                pos.Enfileirar(aux + "");
        }
        string[] posString = pos.ToArray();
        string posfixa = "";
        char caracterAtual = 'A';
        int repeticoes = 0;
        for (int i = 0; i < posString.Length; i++)
        {
            if (IsNumeric(posString[i]))
            {
                posfixa += Convert.ToChar(caracterAtual++) + "";
                for (int rep = 0; rep < repeticoes; rep++)
                    posfixa += "'";
                if (caracterAtual == 91) //acabou o alfabeto
                {
                    repeticoes++;
                    caracterAtual = 'A';
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
        if (i == '(' && j == ')')
            return false;
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
        return int.TryParse(str, out int n);
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