namespace backend.Exceptions;

public class DuplicateNameException : Exception
{
    public DuplicateNameException(string message)
        : base(message) { }
}
