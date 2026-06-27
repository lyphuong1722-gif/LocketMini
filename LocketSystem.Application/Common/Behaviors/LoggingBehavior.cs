using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LocketMini.Application.Common.Behaviors;

/// <summary>
/// Ghi log thời gian xử lý cho mỗi request. Cảnh báo nếu quá 500 ms.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        _logger.LogInformation("[START] Handling {Request}", name);

        var sw = Stopwatch.StartNew();

        try
        {
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
                _logger.LogWarning("[SLOW]  {Request} took {Elapsed}ms", name, sw.ElapsedMilliseconds);
            else
                _logger.LogInformation("[END]   {Request} completed in {Elapsed}ms", name, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "[ERROR] {Request} failed after {Elapsed}ms", name, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
