using Eladei.Architecture.Ddd.Entities;

namespace Eladei.BookRating.Domain.Exceptions;

/// <summary>
/// Книга с указанной информацией уже существует
/// </summary>
public sealed class BookWithCurrentInfoAlreadyExistsException : DomainLogicException
{
    public BookWithCurrentInfoAlreadyExistsException() : base() { }

    public BookWithCurrentInfoAlreadyExistsException(string message) : base(message) { }

    public BookWithCurrentInfoAlreadyExistsException(string format, params object?[] args) : base(string.Format(format, args)) { }

    public BookWithCurrentInfoAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}