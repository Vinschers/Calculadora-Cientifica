using System;

public class Pilha<Dado> : Lista<Dado>, IStack<Dado> where Dado : IComparable<Dado>
{
    public Pilha() : base()
    {
    }
    public void Empilhar(Dado o)
    {
        InserirAntesDoInicio(o);
    }
    public Dado Topo
    {
        get
        {
            if (EstaVazia())
                throw new PilhaVaziaException("Underflow da pilha");
            return primeiro.Info;
        }
    }
    public Dado Pop()
    {
        if (EstaVazia())
            throw new PilhaVaziaException("Underflow da pilha");
        Dado o = primeiro.Info;
        Remove(o);
        return o;
    }
    public int Tamanho()
    {
        return Contar;
    }
    public new bool EstaVazia()
    {
        return base.EstaVazia;
    }
}
