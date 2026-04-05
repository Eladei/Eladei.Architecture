using CqrsWithDddExecuting.DomainModel;

namespace CqrsWithDddExecuting.Infrastructure;

/// <summary>
/// Мок репозитория книг
/// </summary>
public sealed class MockBookRepository : IBookRepository {
    private const string BOOK_NOT_FOUND_ERROR = "Book not found";

    private readonly List<BookInRatingDb> _dataContext;

    public MockBookRepository(List<BookInRatingDb> dataContext) {
        _dataContext = dataContext
            ?? throw new ArgumentNullException(nameof(dataContext));
    }

    public Task SaveBookAsync(BookInRating book, CancellationToken cancellationToken) {
        _dataContext.Add(Convert(book));

        return Task.CompletedTask;
    }

    public Task UpdateBookAsync(BookInRating book, CancellationToken cancellationToken) {
        var updatingBookIndex = _dataContext.FindIndex(b => b.Id == book.Id);
        
        if (updatingBookIndex == -1)
            throw new Exception(BOOK_NOT_FOUND_ERROR);

        _dataContext[updatingBookIndex] = Convert(book);

        return Task.CompletedTask;
    }

    public Task RemoveBookAsync(BookInRating book, CancellationToken cancellationToken) {
        var removingBook = _dataContext.FirstOrDefault(b => b.Id == book.Id)
            ?? throw new Exception(BOOK_NOT_FOUND_ERROR);

        _dataContext.Remove(removingBook);

        return Task.CompletedTask;
    }

    public Task<BookInRating?> FindByIdAsync(Guid bookId, CancellationToken cancellationToken) {
        var foundBook = _dataContext.FirstOrDefault(b => b.Id == bookId);

        var result = foundBook is null 
            ? null 
            : Convert(foundBook);

        return Task.FromResult(result);
    }

    private static BookInRatingDb Convert(BookInRating book)
        => new BookInRatingDb {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Votes = book.Votes
        };

    private static BookInRating Convert(BookInRatingDb bookDb)
        => new BookInRating(bookDb.Id, bookDb.Name, bookDb.Author, bookDb.Votes);
}