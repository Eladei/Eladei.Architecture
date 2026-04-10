using Eladei.Architecture.Ddd.Entities;

namespace Eladei.BookInfo.Domain.Exceptions;

/// <summary>
/// Книга с указанным идентификатором уже существует
/// </summary>
public sealed class BookWithCurrentIdAlreadyExistsException : DomainLogicException
{
    public BookWithCurrentIdAlreadyExistsException() : base() { }

    public BookWithCurrentIdAlreadyExistsException(string message) : base(message) { }

    public BookWithCurrentIdAlreadyExistsException(string format, params object?[] args) : base(string.Format(format, args)) { }

    public BookWithCurrentIdAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}