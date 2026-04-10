using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Eladei.BookRating.Api.Filters;

/// <summary>
/// Перехватчик для логирования входящих запросов
/// </summary>
public sealed class LoggerInterceptor : Interceptor
{
    private static string LoggingMsgPattern
        = "Starting receiving call. Type/Method: {Type} / {Method}";

    private readonly ILogger _logger;

    /// <summary>
    /// Создает объект класса LoggerInterceptor
    /// </summary>
    /// <param name="logger">Логгер</param>
    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation(
            LoggingMsgPattern,
            MethodType.Unary,
            context.Method);

        return await continuation(request, context);
    }
}