using Eladei.Architecture.Cqrs;
using Eladei.BookRating.Application.Commands;
using Eladei.BookRating.Application.Queries;
using Grpc.Core;

namespace Eladei.BookRating.Api.Services;

public sealed class BookRatingServiceV1 : BookRating.BookRatingBase
{
    private readonly IOperationExecutor _operationExecutor;

    public BookRatingServiceV1(IOperationExecutor operationExecutor)
    {
        _operationExecutor = operationExecutor
            ?? throw new ArgumentNullException(nameof(operationExecutor));
    }

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

    public override async Task<UpdateBookApiResponse> UpdateBook(
        UpdateBookApiRequest request,
        ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new UpdateBookInfoCommand(bookId, request.Name, request.Author);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new UpdateBookApiResponse();
    }

    public override async Task<RemoveBookApiResponse> RemoveBook(
        RemoveBookApiRequest request,
        ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new RemoveBookCommand(bookId);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new RemoveBookApiResponse();
    }

    public override async Task<VoteForBookApiResponse> VoteForBook(
        VoteForBookApiRequest request,
        ServerCallContext context)
    {
        var command = new VoteForBookCommand(new Guid(request.BookId));

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new VoteForBookApiResponse();
    }

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
