using System;

public class Pilha<Dado>
{
    public class No<X>
    {
        public No<X> Prox { get; set; }
        public X Info { get; set; }
        public No(No<X> prox, X info)
        {
            Prox = prox;
            Info = info;
        }
    }

    No<Dado> topo;
    public void Empilhar(Dado o)
    {
        if (EstaVazia())
            topo = new No<Dado>(null, o);
        else
            topo = new No<Dado>(topo, o);
    }
    public Dado Topo
    {
        get
        {
            if (EstaVazia())
                throw new PilhaVaziaException("Underflow da pilha");
            return topo.Info;
        }
    }
    public Dado Pop()
    {
        if (EstaVazia())
            throw new PilhaVaziaException("Underflow da pilha");
        Dado o = topo.Info;
        topo = topo.Prox;
        return o;
    }
    public bool EstaVazia()
    {
        return topo == null;
    }
}
