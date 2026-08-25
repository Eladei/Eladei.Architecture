using Eladei.Architecture.Cqrs.EntityFramework.Commands.Exceptions;
using Eladei.Architecture.Cqrs.EntityFramework.Queries.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Eladei.BookInfo.Api.Filters;

public sealed class ErrorInterceptor : Interceptor
{
    private const string ErrorMsgPattern = "Error thrown by {0}";

    private readonly ILogger _logger;

    public ErrorInterceptor(ILogger<ErrorInterceptor> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception exception)
        {
            var ex = exception.InnerException ?? exception;

            switch (ex)
            {
                case OperationCanceledException:
                    throw HandleError(ex, StatusCode.Cancelled, context.Method);
                case TimeoutException:
                    throw HandleError(ex, StatusCode.DeadlineExceeded, context.Method);
                case OverflowException:
                case ArgumentException:
                    throw HandleError(ex, StatusCode.InvalidArgument, context.Method);
                case EfCommandLogicException:
                case EfQueryLogicException:
                    throw HandleError(ex, StatusCode.FailedPrecondition, context.Method);
                case DbModifiedObjectWasRemovedException:
                case DbRemovingObjectWasRemovedException:
                case DbUnknownEntityStateException:
                case CommandExecutionAttemptLimitReachedException:
                    throw HandleError(ex, StatusCode.Aborted, context.Method);
                case DbUpdateConcurrencyException:
                case DbUpdateException:
                    var statusCode = StatusCode.Internal;

                    if (ex is DbUpdateConcurrencyException)
                    {
                        statusCode = StatusCode.Aborted;
                    }

                    throw HandleError(ex, statusCode, context.Method);
                case InvalidOperationException:
                default:
                    throw HandleError(ex, StatusCode.Internal, context.Method);
            }
        }
    }

    private RpcException HandleError(Exception ex, StatusCode statusCode, string methodName)
    {
        _logger.LogError(ex, string.Format(ErrorMsgPattern, methodName));

        // In Release builds, return only the error message
        Status status;
#if DEBUG
        status = new Status(statusCode, ex.ToString());
#else
        status = new Status(statusCode, ex.Message);
#endif

        // TODO: CorrelationId is not included in logs when error is returned
        return new RpcException(status);
    }
}
