using System.ComponentModel.DataAnnotations;

namespace Eladei.BookInfo.Model.Entities;

/// <summary>
/// Базовый класс сущности БД
/// </summary>
public abstract class EntityBase {
    /// <summary>
    /// Версия строки для оптимистической блокировки
    /// </summary>
    public uint Version { get; set; }

    /// <summary>
    /// Дата и время создания в стандарте UTC
    /// </summary>
    [Required]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Дата и время последнего изменения в стандарте UTC
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }
}