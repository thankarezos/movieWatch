using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MovieWatch.Data.Constants;

namespace MovieWatch.Data.Common;

public class ApiResponse<T> : ApiResponse
{

    public T? Data { get; set; }

    public ApiResponse(T? data = default) : base()
    {
        Data = data;
    }
    
    public ApiResponse(ErrorCode error, string message, ValidationResult validationResult) : base(error, message, validationResult)
    { }

}

public class ApiResponse : IActionResult
{
    public bool Success { get; set; } = true;
    public ErrorCode? Error { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }

    public ApiResponse()
    {
        
    }
    

    public ApiResponse(ErrorCode error, string message, ValidationResult validationResult)
    {
        Success = false;
        Message = message;
        Error = error;
        ValidationErrors = validationResult.Errors.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var statusCode = Error.HasValue ? (int) Error : 200;
        var result = new ObjectResult(this)
        {
            StatusCode = statusCode
        };
        return result.ExecuteResultAsync(context);
    }
}