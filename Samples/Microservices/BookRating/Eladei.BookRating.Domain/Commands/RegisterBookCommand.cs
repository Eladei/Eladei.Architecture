using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Application.Properties;
using Eladei.BookRating.Contract.Messaging.IntegrationEvents;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Application.Commands;

public sealed class RegisterBookCommand : EfCommandWithResultBase<BookRatingDbContext, Guid>
{
    private readonly string _name;
    private readonly string _author;

    /// <exception cref="ArgumentException"></exception>
    public RegisterBookCommand(string name, string author)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(Resource.BookNameNotDefined);

        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException(Resource.BookAuthorNotDefined);

        _name = name;
        _author = author;
    }

    /// <returns>Identifier of the created book</returns>
    /// <exception cref="BookWithCurrentInfoAlreadyExistsException"></exception>
    public override async Task<Guid> ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var bookExists = await context.Books
            .AnyAsync(
                s => s.Name == _name && s.Author == _author,
                cancellationToken);

        if (bookExists)
            throw new BookWithCurrentInfoAlreadyExistsException(
                Resource.BookWithCurrentInfoAlreadyExists, _name, _author);

        var newBook = new Book
        {
            Id = Guid.NewGuid(),
            Name = _name,
            Author = _author
        };

        await context.Books.AddAsync(newBook, cancellationToken);

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingIntegrationEvent(
            newBook.Id, newBook.Name, newBook.Author);

        AddIntegrationEvents(bookWasRegisteredEvent);

        return newBook.Id;
    }
}
