namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Неизвестное состояние объекта БД
/// </summary>
public class DbUnknownEntityStateException : Exception {
    public DbUnknownEntityStateException() : base() { }

    public DbUnknownEntityStateException(string message) : base(message) { }

    public DbUnknownEntityStateException(string message, Exception innerException) 
        : base(message, innerException) { }
}