namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Unit of work context factory
/// </summary>
public interface IUnitOfWorkContextFactory
{
    /// <summary>
    /// Creates a unit of work context
    /// </summary>
    /// <returns>The unit of work context</returns>
    IUnitOfWorkContext CreateContext();
}