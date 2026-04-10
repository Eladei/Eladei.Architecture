using Eladei.Architecture.Cqrs.Queries;

namespace Eladei.Architecture.Cqrs.Ddd.Queries;

/// <summary>
/// Страничный запрос
/// </summary>
public abstract class EfPageQueryBase<R> : DddQueryBase<PageResult<R>>
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
    /// Выполнить операцию
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат выполнения операции</returns>
    protected abstract Task<IEnumerable<R>> PerformAsync(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

    /// <summary>
    /// Определить общее количество элементов
    /// </summary>
    /// <param name="repositoryFactory">Фабрика репозиториев</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Общее количество элементов</returns>
    protected abstract Task<uint> GetAllElementsCount(IRepositoryFactory repositoryFactory, CancellationToken cancellationToken);

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