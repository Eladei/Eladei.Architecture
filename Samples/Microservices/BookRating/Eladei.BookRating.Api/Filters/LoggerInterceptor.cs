using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Eladei.BookRating.Api.Filters;

/// <summary>
/// Interceptor for logging incoming requests
/// </summary>
public sealed class LoggerInterceptor : Interceptor
{
    private static string LoggingMsgPattern
        = "Starting receiving call. Type/Method: {Type} / {Method}";

    private readonly ILogger _logger;

    /// <summary>
    /// Creates an instance of the LoggerInterceptor class
    /// </summary>
    /// <param name="logger">Logger instance</param>
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