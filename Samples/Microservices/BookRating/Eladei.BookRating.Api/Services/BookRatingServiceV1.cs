using Eladei.Architecture.Cqrs;
using Eladei.BookRating.Domain.Commands;
using Eladei.BookRating.Domain.Queries;
using Grpc.Core;

namespace Eladei.BookRating.Api.Services;

/// <summary>
/// Service for working with book rating
/// </summary>
public sealed class BookRatingServiceV1 : BookRating.BookRatingBase
{
    private readonly IOperationExecutor _operationExecutor;

    /// <summary>
    /// Creates an instance of the BookRatingServiceV1 class
    /// </summary>
    /// <param name="operationExecutor">Operation executor</param>
    public BookRatingServiceV1(IOperationExecutor operationExecutor)
    {
        _operationExecutor = operationExecutor
            ?? throw new ArgumentNullException(nameof(operationExecutor));
    }

    /// <summary>
    /// Registers a book in the rating
    /// </summary>
    /// <param name="request">Request to add a book to the rating</param>
    /// <param name="context">Server call context</param>
    /// <returns>Response containing the result of the book registration operation</returns>
    public override async Task<RegisterBookApiResponse> RegisterBook(
        RegisterBookApiRequest request,
        ServerCallContext context)
    {
        var command = new RegisterBookCommand(request.Name, request.Author);

        var bookId = await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new RegisterBookApiResponse
        {
            BookId = bookId.ToString()
        };
    }

    /// <summary>
    /// Updates book information in the rating
    /// </summary>
    /// <param name="request">Request to update book information in the rating</param>
    /// <param name="context">Server call context</param>
    /// <returns>Response containing the result of the book update operation</returns>
    public override async Task<UpdateBookApiResponse> UpdateBook(
        UpdateBookApiRequest request,
        ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new UpdateBookInfoCommand(bookId, request.Name, request.Author);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new UpdateBookApiResponse();
    }

    /// <summary>
    /// Removes a book from the rating
    /// </summary>
    /// <param name="request">Request to remove a book from the rating</param>
    /// <param name="context">Server call context</param>
    /// <returns>Response containing the result of the book removal operation</returns>
    public override async Task<RemoveBookApiResponse> RemoveBook(
        RemoveBookApiRequest request,
        ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new RemoveBookCommand(bookId);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new RemoveBookApiResponse();
    }

    /// <summary>
    /// Votes for a book in the rating
    /// </summary>
    /// <param name="request">Request to vote for a book in the rating</param>
    /// <param name="context">Server call context</param>
    /// <returns>Response containing the result of the voting operation</returns>
    public override async Task<VoteForBookApiResponse> VoteForBook(
        VoteForBookApiRequest request,
        ServerCallContext context)
    {
        var command = new VoteForBookCommand(new Guid(request.BookId));

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new VoteForBookApiResponse();
    }

    /// <summary>
    /// Returns a list of books in the rating
    /// </summary>
    /// <param name="request">Request to get books in the rating</param>
    /// <param name="context">Server call context</param>
    /// <returns>Response containing the list of books in the rating</returns>
    public override async Task<GetBooksApiResponse> GetBooks(
        GetBooksApiRequest request,
        ServerCallContext context)
    {
        var query = new BooksQuery(request.BooksPerPage, request.Page);

        var queryResult = await _operationExecutor.ExecuteAsync(query, context.CancellationToken);

        var result = new GetBooksApiResponse
        {
            TotalPages = queryResult.TotalPages
        };

        foreach (var book in queryResult.Result)
        {
            result.AllPositions.Add(new BookInfoApiModel
            {
                BookId = book.Id.ToString(),
                Name = book.Name,
                Author = book.Author,
                Votes = book.Votes,
                RegisteredAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(book.RegisteredAtUtc)
            });
        }

        return result;
    }
}