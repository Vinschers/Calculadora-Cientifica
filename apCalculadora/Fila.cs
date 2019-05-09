using System;
class Fila<Dado>
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
    No<Dado> inicio, fim;
    public void Enfileirar(Dado elemento)
    {
        if (EstaVazia())
            inicio = fim = new No<Dado>(null, elemento);
        else
        {
            fim.Prox = new No<Dado>(null, elemento);
            fim = fim.Prox;
        }
    }
    public Dado Retirar()
    {
        if (!EstaVazia())
        {
            Dado elemento = inicio.Info;
            inicio = inicio.Prox;
            return elemento;
        }
        throw new FilaVaziaException("Fila vazia");
    }
    public Dado Peek()
    {
        if (EstaVazia())
            throw new FilaVaziaException("Fila vazia");
        return inicio.Info;
    }
    public int Tamanho()
    {
        int tamanho = 0;
        for (No<Dado> atual = inicio; atual != null; atual = atual.Prox)
            tamanho++;
        return tamanho;
    }
    public Dado Fim
    {
        get
        {
            if (EstaVazia())
                throw new FilaVaziaException("Fila vazia");

            return fim.Info;
        }
    }
    public bool EstaVazia()
    {
        return inicio == null;
    }
    public Dado[] ToArray()
    {
        Dado[] ret = new Dado[Tamanho()];

        int i = 0;
        for (No<Dado> atual = inicio; atual != null; atual = atual.Prox)
            ret[i++] = atual.Info;

        return ret;
    }
}