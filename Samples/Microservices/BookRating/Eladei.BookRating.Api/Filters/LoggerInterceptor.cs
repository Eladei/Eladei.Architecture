using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Eladei.BookRating.Api.Filters;

public sealed class LoggerInterceptor : Interceptor
{
    private static readonly string LoggingMsgPattern
        = "Starting receiving call. Type/Method: {Type} / {Method}";

    private readonly ILogger _logger;

    public LoggerInterceptor(ILogger<LoggerInterceptor> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
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
