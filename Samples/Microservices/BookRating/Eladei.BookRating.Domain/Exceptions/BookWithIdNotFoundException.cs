using Eladei.Architecture.Ddd.Entities;

namespace Eladei.BookRating.Domain.Exceptions;

/// <summary>
/// A book with the specified identifier was not found
/// </summary>
public sealed class BookWithIdNotFoundException : DomainLogicException
{
    public BookWithIdNotFoundException() : base() { }

    public BookWithIdNotFoundException(string message) : base(message) { }

    public BookWithIdNotFoundException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public BookWithIdNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}