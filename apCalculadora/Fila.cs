using System;
class Fila<Dado> : IQueue<Dado> where Dado : IComparable<Dado>
{
    Lista<Dado> lista;
    public Fila()
    {
        lista = new Lista<Dado>();
    }
    public void Enfileirar(Dado elemento)
    {
        lista.InserirAposFim(elemento);
    }
    public Dado Retirar()
    {
        if (!EstaVazia())
        {
            Dado elemento = lista.Primeiro;
            lista.Remove(elemento);
            return elemento;
        }
        throw new FilaVaziaException("Fila vazia");
    }
    public Dado Peek()
    {
        if (EstaVazia())
            throw new FilaVaziaException("Fila vazia");
        return lista.Primeiro;
    }
    public Dado Fim
    {
        get
        {
            if (EstaVazia())
                throw new FilaVaziaException("Fila vazia");

            return lista.Ultimo;
        }
    }
    public int Tamanho()
    {
        return lista.Contar;
    }
    public bool EstaVazia()
    {
        return lista.EstaVazia;
    }
    public Dado[] ToArray()
    {
        return lista.ToArray();
    }
}