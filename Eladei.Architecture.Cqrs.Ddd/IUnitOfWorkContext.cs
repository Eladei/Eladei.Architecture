namespace Eladei.Architecture.Cqrs.Ddd;

/// <summary>
/// Unit of work context
/// </summary>
public interface IUnitOfWorkContext : IUnitOfWork, IRepositoryFactory { }