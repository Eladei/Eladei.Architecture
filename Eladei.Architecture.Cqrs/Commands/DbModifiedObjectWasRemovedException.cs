namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// The entity being modified has already been deleted from the database
/// </summary>
public class DbModifiedObjectWasRemovedException : Exception
{
    public DbModifiedObjectWasRemovedException() : base() { }

    public DbModifiedObjectWasRemovedException(string message) : base(message) { }

    public DbModifiedObjectWasRemovedException(string message, Exception innerException)
        : base(message, innerException) { }
}