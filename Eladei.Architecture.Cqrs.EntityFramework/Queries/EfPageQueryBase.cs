using Eladei.Architecture.Cqrs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Base paged query for working with Entity Framework
/// </summary>
/// <typeparam name="T">The database context type</typeparam>
/// <typeparam name="R">The result item type</typeparam>
public abstract class EfPageQueryBase<T, R> : EfQueryBase<T, PageResult<R>> where T : DbContext
{
    /// <summary>
    /// The page number to query
    /// </summary>
    protected readonly uint Page;

    /// <summary>
    /// The number of elements per page
    /// </summary>
    protected readonly uint? ElementsPerPage;

    /// <inheritdoc />
    public override async Task<PageResult<R>> ExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        var result = await PerformAsync(context, cancellationToken);

        var pagesAdditionalInfo = await GetPagesAdditionalInfo(context, cancellationToken);

        return new PageResult<R>
        {
            CurrentPage = Page,
            TotalPages = pagesAdditionalInfo.TotalPages,
            TotalElements = pagesAdditionalInfo.TotalElements,
            Result = result
        };
    }

    /// <summary>
    /// Executes the query
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The query result</returns>
    protected abstract Task<IEnumerable<R>> PerformAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total number of elements
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Total number of elements</returns>
    protected abstract Task<uint> GetAllElementsCount(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Number of elements to skip in the query
    /// </summary>
    protected uint ElementsToSkip => ElementsPerPage.HasValue
        ? ElementsPerPage.Value * (Page - 1)
        : 0;

    /// <summary>
    /// Creates a query instance
    /// </summary>
    /// <param name="elementsPerPage">Number of items per page</param>
    /// <param name="page">Page number to retrieve</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected EfPageQueryBase(uint? elementsPerPage = null, uint? page = null)
    {
        if (elementsPerPage.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(elementsPerPage.Value);

        if (page.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(page.Value);

        ElementsPerPage = elementsPerPage;
        Page = page ?? 1;
    }

    private async Task<PageAdditionalInfo> GetPagesAdditionalInfo(T context, CancellationToken cancellationToken)
    {
        var allElementsCount = await GetAllElementsCount(context, cancellationToken);

        var totalPages = ElementsPerPage.HasValue
            ? (uint)Math.Ceiling((double)allElementsCount / ElementsPerPage.Value)
            : allElementsCount;

        return new PageAdditionalInfo
        {
            TotalPages = totalPages,
            TotalElements = allElementsCount
        };
    }

    /// <summary>
    /// Additional paging information
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
