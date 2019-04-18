using System;
class Fila<Dado> : Lista<Dado>, IQueue<Dado> where Dado : IComparable<Dado>, IGravarEmArquivo
{
    public void Enfileirar(Dado elemento)
    {
        base.InserirAposFim(elemento);
    }
    public Dado Retirar()
    {
        if (!EstaVazia())
        {
            Dado elemento = base.primeiro.Info;
            base.Remove(elemento);
            return elemento;
        }
        throw new FilaVaziaException("Fila vazia");
    }
    public Dado Peek()
    {
        if (EstaVazia())
            throw new FilaVaziaException("Fila vazia");
        Dado o = base.primeiro.Info;
        return o;
    }
    public Dado Fim
    {
        get
        {
            if (EstaVazia())
                throw new FilaVaziaException("Fila vazia");

            Dado o = base.ultimo.Info;
            return o;
        }
    }
    public int Tamanho()
    {
        return base.Contar;
    }
    public new bool EstaVazia()
    {
        return base.EstaVazia;
    }
    public new Dado[] ToArray()
    {
        return base.ToArray();
    }
}