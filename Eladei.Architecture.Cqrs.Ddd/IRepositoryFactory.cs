using Eladei.Architecture.Ddd.Repositories;

namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Repository factory
/// </summary>
/// <remarks>
/// Note that the created repository must operate on the same context
/// as the unit of work. This is required to ensure that changes are
/// persisted by the unit of work within a single transaction
/// </remarks>
public interface IRepositoryFactory
{
    /// <summary>
    /// Creates a repository
    /// </summary>
    /// <typeparam name="R">The repository type</typeparam>
    /// <returns>The repository</returns>
    R CreateRepository<R>() where R : IRepository;
}