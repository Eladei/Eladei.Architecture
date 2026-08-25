using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;

namespace Eladei.BookInfo.Application.Exceptions;

public sealed class BookWithCurrentIdAlreadyExistsException : EfCommandLogicException
{
    public BookWithCurrentIdAlreadyExistsException() : base() { }

    public BookWithCurrentIdAlreadyExistsException(string message) : base(message) { }

    public BookWithCurrentIdAlreadyExistsException(string format, params object?[] args) : base(string.Format(format, args)) { }

    public BookWithCurrentIdAlreadyExistsException(string message, Exception inner) : base(message, inner) { }
}
