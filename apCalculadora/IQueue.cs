public interface IQueue<Dado>
{
    void Enfileirar(Dado elemento);
    Dado Retirar();
    Dado Peek();
    int Tamanho();
    bool EstaVazia();
}