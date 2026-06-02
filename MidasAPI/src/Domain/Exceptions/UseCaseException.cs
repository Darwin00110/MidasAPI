namespace MidasAPI;

public class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message) { }
}
