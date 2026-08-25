using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

namespace Eladei.BookRating.Application.Exceptions;

public sealed class BookWithIdNotFoundException : EfCommandLogicException
{
    public BookWithIdNotFoundException() : base() { }

    public BookWithIdNotFoundException(string message) : base(message) { }

    public BookWithIdNotFoundException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public BookWithIdNotFoundException(string message, Exception inner)
        : base(message, inner) { }
}
