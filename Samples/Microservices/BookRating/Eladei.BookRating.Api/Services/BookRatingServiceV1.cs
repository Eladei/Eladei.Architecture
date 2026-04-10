using Eladei.Architecture.Cqrs;
using Eladei.BookRating.Domain.Commands;
using Eladei.BookRating.Domain.Queries;
using Grpc.Core;

namespace Eladei.BookRating.Api.Services;

/// <summary>
/// Сервис работы с рейтингом книг
/// </summary>
public sealed class BookRatingServiceV1 : BookRating.BookRatingBase
{
    private readonly IOperationExecutor _operationExecutor;

    /// <summary>
    /// Создает объект класса BookRatingServiceV1
    /// </summary>
    /// <param name="operationExecutor">Исполнитель операций</param>
    public BookRatingServiceV1(IOperationExecutor operationExecutor)
    {
        _operationExecutor = operationExecutor
            ?? throw new ArgumentNullException(nameof(operationExecutor));
    }

    /// <summary>
    /// Регистрирует книгу в рейтинге
    /// </summary>
    /// <param name="request">Запрос на добавление книги в рейтинг</param>
    /// <param name="context">Контекст для вызова на стороне сервера</param>
    /// <returns>Ответ, содержащий результаты операции 
    /// регистрации книги в рейтинге</returns>
    public override async Task<RegisterBookApiResponse> RegisterBook(RegisterBookApiRequest request, ServerCallContext context)
    {
        var command = new RegisterBookCommand(request.Name, request.Author);

        var bookId = await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new RegisterBookApiResponse
        {
            BookId = bookId.ToString()
        };
    }

    /// <summary>
    /// Обновляет информацию о книге в рейтинге
    /// </summary>
    /// <param name="request">Запрос на изменение информации о книге в рейтинге</param>
    /// <param name="context">Контекст для вызова на стороне сервера</param>
    /// <returns>Ответ, содержащий результаты операции изменения 
    /// информации о книге в рейтинге</returns>
    public override async Task<UpdateBookApiResponse> UpdateBook(UpdateBookApiRequest request, ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new UpdateBookInfoCommand(bookId, request.Name, request.Author);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new UpdateBookApiResponse();
    }

    /// <summary>
    /// Удаляет книгу из рейтинга
    /// </summary>
    /// <param name="request">Запрос на удаление книги из рейтинга</param>
    /// <param name="context">Контекст для вызова на стороне сервера</param>
    /// <returns>Ответ, содержащий результаты операции удаления
    /// книги из рейтинга</returns>
    public override async Task<RemoveBookApiResponse> RemoveBook(RemoveBookApiRequest request, ServerCallContext context)
    {
        var bookId = new Guid(request.BookId);

        var command = new RemoveBookCommand(bookId);

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new RemoveBookApiResponse();
    }

    /// <summary>
    /// Голосует за книгу в рейтинге
    /// </summary>
    /// <param name="request">Запрос на голосование за книгу в рейтинге</param>
    /// <param name="context">Контекст для вызова на стороне сервера</param>
    /// <returns>Ответ, содержащий результаты операции голосования 
    /// за книгу в рейтинге</returns>
    public override async Task<VoteForBookApiResponse> VoteForBook(VoteForBookApiRequest request, ServerCallContext context)
    {
        var command = new VoteForBookCommand(new Guid(request.BookId));

        await _operationExecutor.ExecuteAsync(command, context.CancellationToken);

        return new VoteForBookApiResponse();
    }

    /// <summary>
    /// Возвращает перечень книг, входящих в рейтинг
    /// </summary>
    /// <param name="request">Запрос на получение перечня книг, добавленных в рейтинг</param>
    /// <param name="context">Контекст для вызова на стороне сервера</param>
    /// <returns>Ответ, содержащий перечень книг, добавленных в рейтинг</returns>
    public override async Task<GetBooksApiResponse> GetBooks(GetBooksApiRequest request, ServerCallContext context)
    {
        var query = new BooksQuery(request.BooksPerPage, request.Page);

        var queryResult = await _operationExecutor.ExecuteAsync(query, context.CancellationToken);

        var result = new GetBooksApiResponse
        {
            TotalPages = queryResult.TotalPages
        };

        foreach (var book in queryResult.Result)
        {
            result.AllPositions.Add(new BookInfoApiModel()
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