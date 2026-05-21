using Eladei.Architecture.Cqrs;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Queries;
using Grpc.Core;

namespace Eladei.BookInfo.Api.Services;

/// <summary>
/// Service for working with book information
/// </summary>
public class BookInfoServiceV1 : BookInfo.BookInfoBase
{
    private readonly IOperationExecutor _operationExecutor;

    /// <summary>
    /// Creates an instance of BookInfoServiceV1
    /// </summary>
    /// <param name="operationExecutor">Operation executor</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BookInfoServiceV1(IOperationExecutor operationExecutor)
    {
        _operationExecutor = operationExecutor
            ?? throw new ArgumentNullException(nameof(operationExecutor));
    }

    /// <summary>
    /// Retrieves book information
    /// </summary>
    /// <param name="request">Request containing book identifier</param>
    /// <param name="context">Server call context</param>
    /// <returns>Book information response</returns>
    public override async Task<GetBookInfoApiResponse> GetBookInfo(
        GetBookInfoApiRequest request,
        ServerCallContext context)
    {
        var query = new BookInfoQuery(new Guid(request.BookId));

        var bookInfo = await _operationExecutor.ExecuteAsync(query, context.CancellationToken);

        return new GetBookInfoApiResponse
        {
            BookId = bookInfo.Id.ToString(),
            Name = bookInfo.Name,
            Author = bookInfo.Author,
            Pages = bookInfo.Pages,
            Circulation = bookInfo.Circulation,
            Annotation = bookInfo.Annotation,
            Editor = bookInfo.Editor,
            Translator = bookInfo.Translator,
            Artist = bookInfo.Artist
        };
    }

    /// <summary>
    /// Updates additional book information
    /// </summary>
    /// <param name="request">Request containing additional book information</param>
    /// <param name="context">Server call context</param>
    /// <returns>Update operation result</returns>
    public override async Task<UpdateAdditionalBookInfoApiResponse> UpdateAdditionalBookInfo(
        UpdateAdditionalBookInfoApiRequest request,
        ServerCallContext context)
    {
        var additionalInfo = new AdditionalBookInfo
        {
            Pages = request.Pages,
            Circulation = request.Circulation,
            Annotation = request.Annotation,
            Editor = request.Editor,
            Translator = request.Translator,
            Artist = request.Artist
        };

        var bookId = new Guid(request.BookId);

        var command = new UpdateAdditiotalBookInfoCommand(bookId, additionalInfo);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new UpdateAdditionalBookInfoApiResponse();
    }
}