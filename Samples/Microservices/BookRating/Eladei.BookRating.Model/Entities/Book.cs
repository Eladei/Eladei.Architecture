using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookRating.Model.Entities;

/// <summary>
/// Книга
/// </summary>
[Index(nameof(Name), nameof(Author), IsUnique = true)]
public class Book : EntityBase {
    /// <summary>
    /// Уникальный идентификатор книги
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Название 
    /// </summary>
    [Required]
    [MaxLength(400)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Автор
    /// </summary>
    [Required]
    public string Author { get; set; } = null!;

    /// <summary>
    /// Количество голосов, отданных за книгу
    /// </summary>
    public uint Votes { get; set; }
}