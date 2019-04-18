using System;
using System.Windows.Forms;
public class Lista<Dado> where Dado : IComparable<Dado>
{
    public class NoLista<X>
    {
        private X info; //conteúdo do nó
        private NoLista<X> prox; //contém o ponteiro do procimo nó

        public X Info
        {
            get { return info; }
            set
            {
                if (value != null)
                    info = value;
            }
        } //propriedade do conteúdo
        public NoLista<X> Prox
        {
            get => prox;
            set => prox = value;
        } //propriedade para o ponteiro do proximo nó
        public NoLista()
        { } //construtor vazio (default)
        public NoLista(X info, NoLista<X> prox)
        {
            Info = info;
            Prox = prox;
        } //construtor normal
    } //Listas sempre vão ser de um único tipo

    protected NoLista<Dado> primeiro; //primeiro item da lista
    protected NoLista<Dado> ultimo; //último item da lista
    protected NoLista<Dado> atual; //valor atual
    protected NoLista<Dado> anterior; //valor anterior
    protected int quantosNos; //número de nós

    public delegate bool Procurado(Dado d);


    public Lista()
    {
        primeiro = ultimo = anterior = atual = null;
        quantosNos = 0;
    } //construtor vazio


    public bool EstaVazia
    {
        get => primeiro == null;
    } //retorna se o primeiro é null
    public Dado Primeiro
    {
        get => primeiro.Info;
    } //retorna o primeiro nó
    public Dado Ultimo
    {
        get => ultimo.Info;
    } //retorna o último nó
    public int Contar
    {
        get => quantosNos;
    } //retorna a quantidade de nós
    protected NoLista<Dado> Procurar(Procurado procurado)
    {
        if (EstaVazia)
            return null;
        atual = primeiro;
        while (atual != null)
        {
            if (procurado(atual.Info))
                return atual;
            atual = atual.Prox;
        }
        return null;
    }
    public Dado Achar(Procurado p)
    {
        var ret = Procurar(p);
        if (ret == null)
            return (Dado)(object)ret;
        else
            return ret.Info;
    }
    public void InserirAntesDoInicio(Dado novo)
    {
        if (novo == null)
            throw new ArgumentNullException();
        if (EstaVazia)
            primeiro = ultimo = new NoLista<Dado>(novo, primeiro);
        else
            primeiro = new NoLista<Dado>(novo, primeiro);
        quantosNos++;
    }
    public void InserirAntesDoInicio(NoLista<Dado> no)
    {
        if (no == null || no.Info == null)
            throw new ArgumentNullException();
        if (EstaVazia)
            ultimo = no;
        no.Prox = primeiro;
        primeiro = no;
        quantosNos++;
    } //deprecated
    public void InserirAposFim(Dado novo)
    {
        if (novo == null)
            throw new ArgumentNullException();
        var no = new NoLista<Dado>(novo, null);
        if (EstaVazia)
            primeiro = no;
        else
            ultimo.Prox = no;
        ultimo = no;
        quantosNos++;
    }
    public void Listar(ListBox lsb)
    {
        lsb.Items.Clear();
        for (atual = primeiro; atual != null; atual = atual.Prox)
            lsb.Items.Add(atual.Info);
    }
    public Dado[] ToArray()
    {
        Dado[] itens = new Dado[quantosNos];
        atual = primeiro;
        int i = 0;
        while (atual != null)
        {
            itens[i++] = atual.Info;
            atual = atual.Prox;
        }
        return itens;
    }
    public void Add(Dado d)
    {
        if (d == null)
            throw new ArgumentNullException();
        NoLista<Dado> no = new NoLista<Dado>(d, null);
        if (EstaVazia)
            InserirAntesDoInicio(d);
        else if (!ExisteDado(d))
        {
            if (anterior == null && atual != null)
                InserirAntesDoInicio(d);
            else
            {
                anterior.Prox = no;
                no.Prox = atual;
                if (anterior == ultimo)
                    ultimo = no;
                quantosNos++;
            }
        }
    }
    public bool Remove(Dado d)
    {
        if (d == null)
            throw new ArgumentNullException();
        if (!ExisteDado(d))
            return false;
        else
        {
            if (anterior == null && atual != null)
            {
                primeiro = atual.Prox;
                if (primeiro == null)
                    ultimo = null;
            }
            else
            {
                anterior.Prox = atual.Prox;
                if (atual == ultimo)
                    ultimo = anterior;
            }
            quantosNos--;
            return true;
        }
    }
    public bool Remove(Procurado procurado)
    {
        if (Procurar(procurado) != null)
        {
            if (anterior == null && atual != null)
            {
                primeiro = atual.Prox;
                if (primeiro == null)
                    ultimo = null;
            }
            else
            {
                anterior.Prox = atual.Prox;
                if (atual == ultimo)
                    ultimo = anterior;
            }
            quantosNos--;
            return true;
        }
        return false;
    }
    public void AlterarItem(Procurado p, Dado novo)
    {
        NoLista<Dado> dado = Procurar(p);
        dado.Info = novo;
    }
    public bool ExisteDado(Dado outroProcurado)
    {
        anterior = null;
        atual = primeiro;
        if (EstaVazia)
            return false;
        if (outroProcurado.CompareTo(primeiro.Info) < 0)
            return false;
        if (outroProcurado.CompareTo(ultimo.Info) > 0)
        {
            anterior = ultimo;
            atual = null;
            return false;
        }
        bool achou = false;
        bool fim = false;
        while (!achou && !fim)
        {
            if (atual == null)
                fim = true;
            else
            if (outroProcurado.CompareTo(atual.Info) == 0)
                achou = true;
            else
            if (atual.Info.CompareTo(outroProcurado) > 0)
                fim = true;
            else
            {
                anterior = atual;
                atual = atual.Prox;
            }
        }
        return achou;
    }


    //métodos obrigatórios
    public override string ToString()
    {
        atual = primeiro;
        string ret = "";
        while (atual != null)
        {
            ret += atual.Info.ToString() + "\n";
            atual = atual.Prox;
        }
        return ret;
    }
    public override bool Equals(object obj)
    {
        if (obj == this)
            return true;
        if (obj == null)
            return false;
        if (!obj.GetType().Name.Equals(this.GetType().Name))
            return false;
        var l = (Lista<Dado>)obj;
        var atualThis = primeiro;
        var atualOutra = l.primeiro;
        while (atual != null)
        {
            if (!atualThis.Info.Equals(atualOutra.Info))
                return false;
            atualThis = atualThis.Prox;
            atualOutra = atualOutra.Prox;
        }
        return true;
    }
    public override int GetHashCode()
    {
        int ret = 666;
        atual = primeiro;
        while (atual != null)
        {
            ret = 3 * ret + atual.Info.GetHashCode();
            atual = atual.Prox;
        }
        return ret;
    }
    public object Clone()
    {
        Lista<Dado> l = null;
        try
        {
            l = new Lista<Dado>(this);
        }
        catch (Exception e) { }
        return l;
    }
    public Lista(Lista<Dado> outra)
    {
        if (outra == null)
            throw new ArgumentNullException();
        this.primeiro = outra.primeiro;
        this.ultimo = outra.ultimo;
        this.atual = outra.atual;
        this.anterior = outra.anterior;
        this.quantosNos = outra.quantosNos;
    }
}