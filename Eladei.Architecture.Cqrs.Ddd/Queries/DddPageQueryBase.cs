using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Paged query
/// </summary>
public abstract class EfPageQueryBase<R> : DddQueryBase<PageResult<R>>
{
    private readonly uint _page;
    protected readonly uint? _elementsPerPage;

    /// <summary>
    /// Number of elements to skip for the query
    /// </summary>
    protected uint ElementsToSkip => _elementsPerPage.HasValue
        ? _elementsPerPage.Value * (_page - 1)
        : 0;

    /// <summary>
    /// Creates a new instance of <see cref="EfPageQueryBase{R}"/>
    /// </summary>
    /// <param name="elementsPerPage">The number of elements per page</param>
    /// <param name="page">The page number to query</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected EfPageQueryBase(uint? elementsPerPage = null, uint? page = null)
    {
        if (elementsPerPage.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(elementsPerPage.Value);

        if (page.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(page.Value);

        _elementsPerPage = elementsPerPage;
        _page = page ?? 1;
    }

    public override async Task<PageResult<R>> ExecuteAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken = default)
    {
        var result = await PerformAsync(repositoryFactory, cancellationToken);

        var pagesAdditionalInfo = await GetPagesAdditionalInfo(repositoryFactory, cancellationToken);

        return new PageResult<R>
        {
            CurrentPage = _page,
            TotalPages = pagesAdditionalInfo.TotalPages,
            TotalElements = pagesAdditionalInfo.TotalElements,
            Result = result
        };
    }

    /// <summary>
    /// Executes the query operation
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result items</returns>
    protected abstract Task<IEnumerable<R>> PerformAsync(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total number of elements
    /// </summary>
    /// <param name="repositoryFactory">The repository factory</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The total number of elements</returns>
    protected abstract Task<uint> GetAllElementsCount(
        IRepositoryFactory repositoryFactory,
        CancellationToken cancellationToken);

    private async Task<PageAdditionalInfo> GetPagesAdditionalInfo(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken)
    {
        var allElementsCount = await GetAllElementsCount(repositoryFactory, cancellationToken);

        var totalPages = _elementsPerPage.HasValue
            ? (uint)Math.Ceiling((double)allElementsCount / _elementsPerPage.Value)
            : allElementsCount;

        return new PageAdditionalInfo
        {
            TotalPages = totalPages,
            TotalElements = allElementsCount
        };
    }

    /// <summary>
    /// Paging metadata
    /// </summary>
    private record PageAdditionalInfo
    {
        /// <summary>
        /// Total number of pages
        /// </summary>
        public uint TotalPages { get; init; }

        /// <summary>
        /// Total number of elements across all pages
        /// </summary>
        public uint TotalElements { get; init; }
    }
}