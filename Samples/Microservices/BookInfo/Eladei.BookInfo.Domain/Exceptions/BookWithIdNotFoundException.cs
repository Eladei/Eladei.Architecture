using Eladei.Architecture.Ddd.Entities;

namespace Eladei.BookInfo.Domain.Exceptions;

/// <summary>
/// Книга с указанным идентификатором не найдена
/// </summary>
public sealed class BookWithIdNotFoundException : DomainLogicException
{
    public BookWithIdNotFoundException() : base() { }

    public BookWithIdNotFoundException(string message) : base(message) { }

    public BookWithIdNotFoundException(string format, params object?[] args) : base(string.Format(format, args)) { }

    public BookWithIdNotFoundException(string message, Exception inner) : base(message, inner) { }
}