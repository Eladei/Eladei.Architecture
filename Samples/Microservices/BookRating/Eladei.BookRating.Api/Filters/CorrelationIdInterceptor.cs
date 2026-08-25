using Eladei.Architecture.Logging;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Eladei.BookRating.Api.Filters;

/// <summary>
/// Interceptor that sets correlationId if it was not provided by the client
/// </summary>
/// <remarks>
/// CorrelationId is set only if it was not sent by the client via the "x-correlation-id" header.
/// </remarks>
public sealed class CorrelationIdInterceptor : Interceptor
{
    private readonly ICorrelationContext _correlationContext;

    /// <exception cref="ArgumentNullException"></exception>
    public CorrelationIdInterceptor(ICorrelationContext correlationContext)
    {
        _correlationContext = correlationContext
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    /// <inheritdoc />
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        string? headerCorrelationId = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "x-correlation-id")?.Value;

        var correlationId = string.IsNullOrEmpty(headerCorrelationId)
            ? Guid.NewGuid()
            : new Guid(headerCorrelationId);

        using (_correlationContext.SetCorrelationId(correlationId))
        {
            return await continuation.Invoke(request, context);
        }
    }
}
