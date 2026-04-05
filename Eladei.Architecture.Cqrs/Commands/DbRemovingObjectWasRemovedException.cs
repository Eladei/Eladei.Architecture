namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Удаляемый в БД объект был уже удален
/// </summary>
public class DbRemovingObjectWasRemovedException : Exception {
    public DbRemovingObjectWasRemovedException() : base() { }

    public DbRemovingObjectWasRemovedException(string message) : base(message) { }

    public DbRemovingObjectWasRemovedException(string message, Exception innerException) 
        : base(message, innerException) { }
}