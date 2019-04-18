using System;
class FilaVaziaException : Exception
{
    public FilaVaziaException(string err) : base(err)
    {
    }
}
