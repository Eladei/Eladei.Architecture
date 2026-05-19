namespace Eladei.Architecture.Cqrs.Commands;

/// <summary>
/// The entity being deleted has already been removed from the database
/// </summary>
public class DbRemovingObjectWasRemovedException : Exception
{
    public DbRemovingObjectWasRemovedException() : base() { }

    public DbRemovingObjectWasRemovedException(string message) : base(message) { }

    public DbRemovingObjectWasRemovedException(string message, Exception innerException)
        : base(message, innerException) { }
}