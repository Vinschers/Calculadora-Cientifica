using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Calculadora
{
    private class Conta
    {
        public string Infixa { get; set; }
        public string Posfixa { get; set; }
        public double Resultado { get; set; }
    }
    Pilha<Conta> contas;

    public string Infixa { get => contas.Topo.Infixa; set => contas.Topo.Infixa = value; }
    public string Posfixa { get => contas.Topo.Posfixa; }

    public Calculadora()
    {
        contas = new Pilha<Conta>();
    }
    public double CalcularExpressao()
    {
        double result = contas.Topo.Resultado;
        contas.Empilhar(new Conta());
        return result;
    }
    private void CalcularPosfixa()
    {
        throw new NotImplementedException();
    }
    private bool HaPrecedencia(char i, char j)
    {
        throw new NotImplementedException();
    }
}