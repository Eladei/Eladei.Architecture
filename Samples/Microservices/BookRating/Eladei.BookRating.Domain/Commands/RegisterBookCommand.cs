using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Domain.Commands.DomainEvents;
using Eladei.BookRating.Domain.Exceptions;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Eladei.BookRating.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Commands;

/// <summary>
/// Command for registering a book in the rating
/// </summary>
public sealed class RegisterBookCommand : EfCommandWithResultBase<BookRatingDbContext, Guid>
{
    private readonly string _name;
    private readonly string _author;

    /// <summary>
    /// Creates an instance of the RegisterBookCommand class
    /// </summary>
    /// <param name="name">Book title</param>
    /// <param name="author">Book author</param>
    /// <exception cref="ArgumentException"></exception>
    public RegisterBookCommand(string name, string author)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException(Resource.BookNameNotDefined);

        if (string.IsNullOrEmpty(author))
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

        var bookWasRegisteredEvent = new BookWasRegisteredInRatingDomainEvent(
            newBook.Id, newBook.Name, newBook.Author);

        SaveDomainEvents(bookWasRegisteredEvent);

        return newBook.Id;
    }
}