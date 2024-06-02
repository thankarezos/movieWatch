using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MovieWatch.Data.Common;
using MovieWatch.Data.Constants;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Filters;

public class AuthorizationAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userService = context.HttpContext.RequestServices.GetService<IUserService>();
        var headers = context.HttpContext.Request.Headers;

        var validationResult = new ValidationResult();

        if (!headers.ContainsKey("Authorization") || string.IsNullOrWhiteSpace(headers["Authorization"]))
        {
            validationResult.Errors.Add(new ValidationFailure("Authorization", "Authorization header is missing"));
            context.Result = new JsonResult(new ApiResponse(ErrorCode.Unauthorized, "Unauthorized", validationResult));
            return;
        }

        var user = await userService!.GetUserFromToken(headers.Authorization!);

        if (user == null)
        {
            validationResult.Errors.Add(new ValidationFailure("Authorization", "Invalid token"));
            context.Result = new ApiResponse(ErrorCode.Unauthorized, "Unauthorized", validationResult);
            return;
        }
        
        context.HttpContext.Items["User"] = user;

        await next();
    }
}