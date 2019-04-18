using System;

public interface IStack<Dado>
{
    void Empilhar(Dado elemento);
    Dado Pop();
    bool EstaVazia();
    int Tamanho();
}
