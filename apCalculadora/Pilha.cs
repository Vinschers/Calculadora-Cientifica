using System;

public class Pilha<Dado> : IStack<Dado> where Dado : IComparable<Dado>
{
    Lista<Dado> lista;
    public Pilha()
    {
        lista = new Lista<Dado>();
    }
    public void Empilhar(Dado o)
    {
        lista.InserirAntesDoInicio(o);
    }
    public Dado Topo
    {
        get
        {
            if (EstaVazia())
                throw new PilhaVaziaException("Underflow da pilha");
            return lista.Primeiro;
        }
    }
    public Dado Base
    {
        get
        {
            if (EstaVazia())
                throw new PilhaVaziaException("Underflow da pilha");
            return lista.Ultimo;
        }
    }
    public Dado Pop()
    {
        if (EstaVazia())
            throw new PilhaVaziaException("Underflow da pilha");
        Dado o = lista.Primeiro;
        lista.Remove(o);
        return o;
    }
    public int Tamanho()
    {
        return lista.Contar;
    }
    public bool EstaVazia()
    {
        return lista.EstaVazia;
    }
}
