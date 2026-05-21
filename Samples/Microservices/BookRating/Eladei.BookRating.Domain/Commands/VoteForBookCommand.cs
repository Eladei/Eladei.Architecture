using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Domain.Exceptions;
using Eladei.BookRating.Domain.Properties;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Domain.Commands;

/// <summary>
/// Command for voting for a book in the rating
/// </summary>
public sealed class VoteForBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

    /// <summary>
    /// Creates an instance of the VoteForBookCommand class
    /// </summary>
    /// <param name="bookId">Book identifier</param>
    public VoteForBookCommand(Guid bookId)
    {
        _bookId = bookId;
    }

    /// <exception cref="BookWithIdNotFoundException"></exception>
    public override async Task ExecuteAsync(BookRatingDbContext context, CancellationToken cancellationToken)
    {
        var book = await context.Books.FirstOrDefaultAsync(s => s.Id == _bookId, cancellationToken)
            ?? throw new BookWithIdNotFoundException(Resource.BookWithIdNotFound, _bookId);

        book.Votes++;

        context.Books.Update(book);
    }
}