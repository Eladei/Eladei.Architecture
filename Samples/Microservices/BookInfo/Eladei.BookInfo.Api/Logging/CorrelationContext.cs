using Eladei.Architecture.Logging;
using Serilog.Context;

namespace Eladei.BookInfo.Api.Logging;

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
