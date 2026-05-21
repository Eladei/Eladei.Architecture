using Eladei.Architecture.Logging;
using Serilog.Context;

namespace Eladei.BookRating.Api.Logging;

/// <summary>
/// Correlation context for distributed tracing and logging
/// </summary>
/// <remarks>
/// CorrelationId is propagated through the entire asynchronous call chain via AsyncLocal.
/// The value can only be changed using SetCorrelationId
/// and requires proper lifetime management via IDisposable (use using).
/// </remarks>
public class CorrelationContext : ICorrelationContext
{
    private const string CORRELATION_ID_PROPERTY = "CorrelationId";

    private static readonly AsyncLocal<Guid> _correlationId = new();

    public IDisposable SetCorrelationId(Guid correlationId)
    {
        _correlationId.Value = correlationId;

        return LogContext.PushProperty(CORRELATION_ID_PROPERTY, correlationId);
    }

    public Guid CorrelationId => _correlationId.Value;
}