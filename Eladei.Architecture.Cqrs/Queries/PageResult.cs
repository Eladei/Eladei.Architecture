namespace Eladei.Architecture.Cqrs.Queries;

/// <summary>
/// Paged search result
/// </summary>
/// <typeparam name="T">The type of the items</typeparam>
public class PageResult<T>
{
    /// <summary>
    /// Current page number
    /// </summary>
    public uint CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public uint TotalPages { get; set; }

    /// <summary>
    /// Total number of elements across all pages
    /// </summary>
    public uint TotalElements { get; set; }

    /// <summary>
    /// Items on the page
    /// </summary>
    public required IEnumerable<T> Result { get; set; }
}