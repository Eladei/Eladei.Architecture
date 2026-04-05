namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// Изменяемый в БД объект был уже удален
/// </summary>
public class DbModifiedObjectWasRemovedException : Exception {
    public DbModifiedObjectWasRemovedException() : base() { }

    public DbModifiedObjectWasRemovedException(string message) : base(message) { }

    public DbModifiedObjectWasRemovedException(string message, Exception innerException) 
        : base(message, innerException) { }
}