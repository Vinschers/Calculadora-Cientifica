using System;

public interface IStack<Dado> where Dado : IComparable<Dado>
{
    void Empilhar(Dado elemento);
    Dado Pop();
    Dado Topo { get; }
    bool EstaVazia();
    int Tamanho();
}
