public interface IQueue<Dado>
{
    void Enfileirar(Dado elemento);
    Dado Retirar();
    Dado Peek();
    Dado Fim { get; }
    int Tamanho();
    bool EstaVazia();
}