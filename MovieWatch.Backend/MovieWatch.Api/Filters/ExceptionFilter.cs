using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MovieWatch.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(IWebHostEnvironment environment, ILogger<ExceptionFilter> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;
        var includeStackTrace = _environment.IsDevelopment(); // Or use a configuration setting
        var result = new ObjectResult(new
        {
            message = "An unexpected error occurred. Please try again later.",
            error = includeStackTrace ? context.Exception.Message : null,
            stackTrace = includeStackTrace ? context.Exception.StackTrace : null
        })
        {
            StatusCode = 500
        };
        
        _logger.LogError(context.Exception, context.Exception.Message);

        context.Result = result;
        context.ExceptionHandled = true;
    }
}