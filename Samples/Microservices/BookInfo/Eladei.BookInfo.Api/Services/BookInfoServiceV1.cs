using Eladei.Architecture.Cqrs;
using Eladei.BookInfo.Domain.Commands;
using Eladei.BookInfo.Domain.Queries;
using Grpc.Core;

namespace Eladei.BookInfo.Api.Services;

/// <summary>
/// Сервис работы с информацией о книгах
/// </summary>
public class BookInfoServiceV1 : BookInfo.BookInfoBase
{
    private readonly IOperationExecutor _operationExecutor;

    public BookInfoServiceV1(IOperationExecutor operationExecutor)
    {
        _operationExecutor = operationExecutor
            ?? throw new ArgumentNullException(nameof(operationExecutor));
    }

    public override async Task<GetBookInfoApiResponse> GetBookInfo(GetBookInfoApiRequest request, ServerCallContext context)
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

    public override async Task<UpdateAdditionalBookInfoApiResponse> UpdateAdditionalBookInfo(UpdateAdditionalBookInfoApiRequest request, ServerCallContext context)
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