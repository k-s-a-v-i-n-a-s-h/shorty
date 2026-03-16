using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Shorty.Management.Api.Handlers;

public class ExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemService;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(IProblemDetailsService problemService, ILogger<ExceptionHandler> logger)
    {
        _problemService = problemService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is BadHttpRequestException)
        {
            return await _problemService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Invalid JSON format"
                }
            });
        }

        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        _logger.LogError(exception, "An unhandled server error occurred. TraceId: {TraceId}", traceId);

        return await _problemService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred"
            }
        });
    }
}
