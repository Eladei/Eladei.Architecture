using Microsoft.EntityFrameworkCore;
using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.EntityFramework.Queries;

/// <summary>
/// Запрос для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Контекст данных</typeparam>
public abstract class EfPageQueryBase<T, R> : EfQueryBase<T, PageResult<R>> where T : DbContext
{
    private readonly uint _page;
    protected readonly uint? _elementsPerPage;

    /// <summary>
    /// Количество пропускаемых элементов при запросе
    /// </summary>
    protected uint ElementsToSkip => _elementsPerPage.HasValue
        ? _elementsPerPage.Value * (_page - 1)
        : 0;

    /// <summary>
    /// Создает объект класса Query
    /// </summary>
    /// <param name="elementsPerPage">Число элементов на страницу</param>
    /// <param name="page">Номер страницы, для которой будет вестись поиск</param>>
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

    public override async Task<PageResult<R>> ExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        var result = await PerformAsync(context, cancellationToken);

        var pagesAdditionalInfo = await GetPagesAdditionalInfo(context, cancellationToken);

        return new PageResult<R>
        {
            CurrentPage = _page,
            TotalPages = pagesAdditionalInfo.TotalPages,
            TotalElements = pagesAdditionalInfo.TotalElements,
            Result = result
        };
    }

    /// <summary>
    /// Выполнить операцию
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    protected abstract Task<IEnumerable<R>> PerformAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Определить общее количество элементов
    /// </summary>
    /// <param name="context">Контекст данных</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Общее количество элементов</returns>
    protected abstract Task<uint> GetAllElementsCount(T context, CancellationToken cancellationToken);

    private async Task<PageAdditionalInfo> GetPagesAdditionalInfo(T context, CancellationToken cancellationToken)
    {
        var allElementsCount = await GetAllElementsCount(context, cancellationToken);

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
    /// Дополнительная информация о странице
    /// </summary>
    private record PageAdditionalInfo
    {
        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public uint TotalPages { get; init; }

        /// <summary>
        /// Общее количество элементов на всех страницах
        /// </summary>
        public uint TotalElements { get; init; }
    }
}