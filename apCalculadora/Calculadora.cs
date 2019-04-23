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
    public double Resultado { get => contas.Topo.Resultado; }

    public Calculadora()
    {
        contas = new Pilha<Conta>();
        precedencia = new bool[9, 9];
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

        while (!posfixa.EstaVazia()) // Percorre a pilha da sequência posfixa, realizando as operações
        {
            string atual = posfixa.Retirar();
            if (IsNumeric(atual))
                resultados.Empilhar(double.Parse(atual));
            else
            {
                if (IsUnary(atual))
                {
                    double operando = resultados.Pop();
                    double resultadoAtual = RealizarOperacaoUnaria(operando, atual);
                    resultados.Empilhar(resultadoAtual);
                }
                else
                {
                    double segundo = resultados.Pop();
                    double primeiro = resultados.Pop();
                    double resultadoAtual = RealizarOperacaoBinaria(primeiro, segundo, atual);
                    resultados.Empilhar(resultadoAtual);
                }
            }
        }

        result = resultados.Pop(); // Após o percurso, o único elemento restante na pilha de resultados é o próprio resultado
        contas.Topo.Resultado = result;
        return result;
    }
    public void IniciarNovaConta()
    {
        contas.Empilhar(new Conta());
    }
    private Fila<string> CalcularPosfixa()
    {
        string[] infixa = contas.Topo.Infixa.Split(' ');

        Pilha<string> pilha = new Pilha<string>(); // Pilha de operadores
        Fila<string> pos = new Fila<string>(); // Sequência posfixa

        for (int i = 0; i < infixa.Length; i++)
        {
            string atual = infixa[i];

            if (IsNumeric(atual)) // Sempre que um número é encontrado, ele é colocado na sequência
                pos.Enfileirar(atual);
            else
            {
                while (!pilha.EstaVazia() && HaPrecedencia(pilha.Topo[0], atual[0])) // Coloca os operadores da pilha na sequência enquanto seu topo tiver precedência sobre o operador lido
                {
                    string operadorAtual = pilha.Pop().ToString();
                    if (!operadorAtual.Equals("(") && !operadorAtual.Equals(")"))
                        pos.Enfileirar(operadorAtual);
                }
                if (!pilha.EstaVazia() && pilha.Topo == "(" && atual[0] == ')') // Se o operador lido for um fecha parênteses, a pilha terá sido percorrida até encontrar um abre, que deve ser removido
                    pilha.Pop();
                else if (atual[0] != ')') // Se não for, como o operador lido tem preferência, ele deve ser empilhado
                    pilha.Empilhar(atual);
            }
        }
        while (!pilha.EstaVazia()) // Se não há mais nenhum operador para ler, mas a pilha ainda tem elementos, ela deve ser descarregada
        {
            string aux = pilha.Pop();
            if (aux != "(" && aux != ")")
                pos.Enfileirar(aux + "");
        }
        string[] posString = pos.ToArray();

        string posfixa = "";
        char caracterAtual = 'A';
        int repeticoes = 0;

        for (int i = 0; i < posString.Length; i++) // Transforma a sequência de números em letras.
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
        else if (i == '!')
            i = Convert.ToChar(i + 6);
        else if (i == 'l')
            i = Convert.ToChar(i - 61);
        else if (i == '√')
            i = Convert.ToChar(i - 8683);

        if (j == '-')
            j = Convert.ToChar(j - 1);
        else if (j == '/')
            j = Convert.ToChar(j - 2);
        else if (j == '^')
            j = Convert.ToChar(j - 48);
        else if (j == '!')
            j = Convert.ToChar(j + 6);
        else if (j == 'l')
            j = Convert.ToChar(j - 61);
        else if (j == '√')
            j = Convert.ToChar(j - 8683);

        i = Convert.ToChar(i - 39);
        j = Convert.ToChar(j - 39);

        return precedencia[i, j];
    }
    private bool IsNumeric(string str)
    {
        return double.TryParse(str, out double n);
    }
    private bool IsUnary(string o)
    {
        return o == "!" || o == "log" || o == "√";
    }
    private double RealizarOperacaoUnaria(double a, string o)
    {
        switch (o)
        {
            case "!":
                if (a != Math.Abs(Math.Floor(a)))
                    throw new Exception("Um número precisa ser inteiro e natural para realizar fatorial!");
                double resultado = 1;
                while (a > 0)
                {
                    resultado *= a;
                    a--;
                }
                return resultado;
            case "log":
                return Math.Log10(a);
            case "√":
                return Math.Sqrt(a);
            default:
                return 0;
        }
    }
    private double RealizarOperacaoBinaria(double a, double b, string o)
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