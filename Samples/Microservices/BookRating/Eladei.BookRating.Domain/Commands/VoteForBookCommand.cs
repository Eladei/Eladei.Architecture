using Eladei.Architecture.Cqrs.EntityFramework.Commands;
using Eladei.BookRating.Application.Exceptions;
using Eladei.BookRating.Application.Properties;
using Eladei.BookRating.Model;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookRating.Application.Commands;

public sealed class VoteForBookCommand : EfCommandBase<BookRatingDbContext>
{
    private readonly Guid _bookId;

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
