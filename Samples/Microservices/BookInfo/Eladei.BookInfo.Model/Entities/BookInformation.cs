using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

/// <summary>
/// Информация о книге
/// </summary>
[Index(nameof(Name), nameof(Author), IsUnique = true)]
public class BookInformation : EntityBase {
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
    /// Число страниц
    /// </summary>
    public uint? Pages { get; set; }

    /// <summary>
    /// Тираж
    /// </summary>
    public uint? Circulation { get; set; }

    /// <summary>
    /// Аннотация
    /// </summary>
    [MaxLength(1000)]
    public string? Annotation { get; set; } = null!;

    /// <summary>
    /// Редактор
    /// </summary>
    [MaxLength(100)]
    public string? Editor { get; set; }

    /// <summary>
    /// Переводчик
    /// </summary>
    [MaxLength(100)]
    public string? Translator { get; set; }

    /// <summary>
    /// Художник
    /// </summary>
    [MaxLength(100)]
    public string? Artist { get; set; }
}