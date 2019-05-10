using System;
using System.IO;
using System.Text.RegularExpressions;

public class Calculadora
{
    private class Conta : IComparable<Conta>
    {
        private bool IsNumeric(string str)
        {
            return double.TryParse(str, out double n);
        }
        public string[] Infixa { get; set; }
        public string Posfixa { get; set; }
        public double Resultado { get; set; }
        public int CompareTo(Conta outra)
        {
            return Resultado.CompareTo(outra.Resultado);
        }
        public int QtdElementos { get; set; }
        public Conta()
        {
            Infixa = new string[10];
            Posfixa = "";
            Resultado = 0;
        }
        public void Adicionar(string v) // (
        {
            if (QtdElementos > 0 && (IsNumeric(v) || v == "," || v == ".") && IsNumeric(Infixa[QtdElementos - 1]))
            {
                Infixa[QtdElementos - 1] += v;
            }
            else
            {
                if (QtdElementos > 0 && Infixa[QtdElementos - 1] == "-" && (QtdElementos == 1 || (QtdElementos > 1 && Infixa[QtdElementos - 2] == "(")))
                    Infixa[QtdElementos - 1] += v;
                else
                {
                    Infixa[QtdElementos++] = v;
                    if (QtdElementos == Infixa.Length)
                        AumentarVetor();
                }
            }
        }
        private void AumentarVetor()
        {
            string[] novo = new string[(int)Math.Floor(Infixa.Length * 1.5)];
            for (int i = 0; i < Infixa.Length; i++)
                novo[i] = Infixa[i];
            Infixa = novo;
        }
        public string Excluir()
        {
            string ret = Infixa[--QtdElementos];
            Infixa[QtdElementos] = null;
            return ret;
        }
    }
    private bool[,] precedencia;
    Pilha<Conta> contas;

    public void AdicionarAoVetor(string valor)
    {
        if (!contas.EstaVazia())
            contas.Topo.Adicionar(valor);
    }
    public string ExcluirDoVetor()
    {
        if (!contas.EstaVazia())
            return contas.Topo.Excluir();
        return "";
    }
    public string UltimoTermo()
    {
        if (!contas.EstaVazia())
            return contas.Topo.Infixa[contas.Topo.QtdElementos - 1];
        return "";
    }
    public void AlterarUltimoTermo(string v)
    {
        if (!contas.EstaVazia())
            contas.Topo.Infixa[contas.Topo.QtdElementos - 1] = v;
    }
    public string[] Infixa
    {
        get
        {
            if (!contas.EstaVazia())
                return contas.Topo.Infixa;
            return null;
        }
        set
        {
            if (!contas.EstaVazia())
                contas.Topo.Infixa = value;
        }
    }
    public int QtdElementosUltimaConta
    {
        get => contas.Topo.QtdElementos;
    }
    public void ExcluirVetor()
    {
        if (!contas.EstaVazia())
            while (contas.Topo.QtdElementos > 0)
                contas.Topo.Excluir();
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
    public string[] GerarInfixaComLetras()
    {
        string[] ret = new string[QtdElementosUltimaConta];

        char letra = 'A';
        int repeticoes = 0;
        for (int i = 0; i < QtdElementosUltimaConta; i++)
            if (IsNumeric(Infixa[i]))
            {
                ret[i] = letra++.ToString();
                for (int j = 0; j < repeticoes; j++)
                    ret[i] += "'";
                if (letra == 'Z')
                {
                    repeticoes++;
                    letra = 'A';
                }
            }
            else
                ret[i] = Infixa[i];

        return ret;
    }
    public double CalcularExpressao()
    {
        string[] infixaComLetras = GerarInfixaComLetras();
        Fila<string> posfixa = CalcularPosfixa(infixaComLetras);
        Pilha<double> resultados = new Pilha<double>();
        double result = 0;

        while (!posfixa.EstaVazia()) // Percorre a fila da sequência posfixa, realizando as operações
        {
            string atual = posfixa.Retirar();
            if (IsLetter(atual))
            {
                int nNumero = atual[0] + 26 * (atual.Length - 1) - 'A'; //é o n-ésimo numero, comecando no 0
                int qtdNumerosPassados = 0;
                for (int i = 0; i < QtdElementosUltimaConta; i++)
                {
                    if (IsNumeric(Infixa[i]))
                    {
                        if (nNumero == qtdNumerosPassados)
                        {
                            resultados.Empilhar(double.Parse(Infixa[i]));
                            break;
                        }
                        qtdNumerosPassados++;
                    }
                }
            }
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
        if (!resultados.EstaVazia())
            result = resultados.Pop(); // Após o percurso, o único elemento restante na pilha de resultados é o próprio resultado
        else
            return 0;
        contas.Topo.Resultado = result;
        return result;
    }
    public void IniciarNovaConta()
    {
        contas.Empilhar(new Conta());
    }
    private Fila<string> CalcularPosfixa(string[] infixa)
    {
        if (infixa[0] == null)
            return new Fila<string>();

        Pilha<string> pilha = new Pilha<string>(); // Pilha de operadores
        Fila<string> pos = new Fila<string>(); // Sequência posfixa

        for (int i = 0; i < contas.Topo.QtdElementos; i++)
        {
            string atual = infixa[i];

            if (IsLetter(atual)) // Sempre que um número é encontrado, ele é colocado na sequência
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

        string posfixa = String.Join("", posString);
       
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
    public bool IsLetter(string str)
    {
        return str.Length > 0 && str[0] >= 'A' && str[0] <= 'Z';
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
                if (a < 0)
                    throw new Exception("Logaritmo de número negativo!");
                return Math.Log10(a);
            case "√":
                if (a < 0)
                    throw new Exception("Raiz de número negativo!");
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
                if (b != 0 && a != 0)
                    return a / b;
                if (b == 0 && a != 0)
                    throw new Exception("Erro - não é possível dividir por 0");
                else
                    throw new Exception("Indefinido");
            case "^":
                return Math.Pow(a, b);
            default:
                return 0;
        }
    }
}