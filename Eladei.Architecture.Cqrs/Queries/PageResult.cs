namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Страничный результат поиска
/// </summary>
/// <typeparam name="T">Тип найденных элементов</typeparam>
public class PageResult<T> {
    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public uint CurrentPage { get; set; }

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public uint TotalPages { get; set; }

    /// <summary>
    /// Общее количество элементов на всех страницах
    /// </summary>
    public uint TotalElements { get; set; }

    /// <summary>
    /// Элементы на странице
    /// </summary>
    public required IEnumerable<T> Result { get; set; }
}