using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

namespace Eladei.BookRating.Application.Exceptions;

public sealed class BookWithCurrentInfoAlreadyExistsException : EfCommandLogicException
{
    public BookWithCurrentInfoAlreadyExistsException() : base() { }

    public BookWithCurrentInfoAlreadyExistsException(string message) : base(message) { }

    public BookWithCurrentInfoAlreadyExistsException(string format, params object?[] args)
        : base(string.Format(format, args)) { }

    public BookWithCurrentInfoAlreadyExistsException(string message, Exception inner)
        : base(message, inner) { }
}
