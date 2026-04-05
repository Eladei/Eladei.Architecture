using Eladei.Architecture.Logging;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Eladei.BookRating.Api.Filters;

/// <summary>
/// Перехватчик для установки correlationId, если он не был отправлен с клиента
/// </summary>
public sealed class CorrelationIdInterceptor : Interceptor {
    private readonly ICorrelationContext _correlationContext;

    public CorrelationIdInterceptor(ICorrelationContext correlationContext) {
        _correlationContext = correlationContext 
            ?? throw new ArgumentNullException(nameof(correlationContext));
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation) {
        string? headerCorrelationId = context.RequestHeaders
            .FirstOrDefault(h => h.Key == "x-correlation-id")?.Value;

        var correlationId = string.IsNullOrEmpty(headerCorrelationId) 
            ? Guid.NewGuid() 
            : new Guid(headerCorrelationId);

        using (_correlationContext.SetCorrelationId(correlationId)) {
            return await continuation.Invoke(request, context);
        }
    }
}