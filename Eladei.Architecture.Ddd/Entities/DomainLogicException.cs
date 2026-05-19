namespace Eladei.Architecture.Ddd.Entities;

/// <summary>
/// Exception thrown when a domain rule is violated
/// </summary>
public class DomainLogicException : Exception
{
    public DomainLogicException() { }

    public DomainLogicException(string message) : base(message) { }

    public DomainLogicException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public DomainLogicException(string message, Exception inner)
        : base(message, inner) { }
}