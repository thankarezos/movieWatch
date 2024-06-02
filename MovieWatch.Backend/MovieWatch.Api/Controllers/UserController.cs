using FluentValidation.Results;
using MetroClimate.Data.Dtos.Payload;
using Microsoft.AspNetCore.Mvc;
using MovieWatch.Data.Common;
using MovieWatch.Data.Constants;
using MovieWatch.Data.Database;
using MovieWatch.Data.Dtos;
using MovieWatch.Data.Pld;
using MovieWatch.Data.Response;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidationService _validationService;
    
    public UserController(IUserService userService, IValidationService validationService)
    {
        _userService = userService;
        _validationService = validationService;
    }
    [HttpPost("login")]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginPld loginPld)
    {
        var validationResponse = await _validationService.ValidatePldAsync<LoginPld, LoginResponse>(loginPld);
        if (validationResponse != null) return validationResponse;
        
        var (token, user) = await _userService.Login(loginPld.Username, loginPld.Password);
        
        if (token == null || user == null)
        {
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("username", "Invalid username or password"));
            return new ApiResponse<LoginResponse>(ErrorCode.Unauthorized, "Invalid data", validationResult);
        }
        
        var userDto = new UserDto(user!);
        
        return new ApiResponse<LoginResponse>(new LoginResponse
        {
            Token = token!,
            User = userDto
        });
        
        
        
    }
    
    [HttpPost("register")]
    public async Task<ApiResponse<LoginResponse>> Register([FromBody] RegisterPld registerPld)
    {
        var validatorResponse = await _validationService.ValidatePldAsync<RegisterPld, LoginResponse>(registerPld);
        if (validatorResponse != null) return validatorResponse;
        
        var (token, user) = await _userService.Register(registerPld);
        
        if (token == null || user == null)
        {
            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Username", "Username already exists"));
            return new ApiResponse<LoginResponse>(ErrorCode.BadRequest, "Invalid data", validationResult);
        }
        
        var userDto = new UserDto(user!);
        
        return new ApiResponse<LoginResponse>(new LoginResponse
        {
            Token = token!,
            User = userDto
        });
    }
    
}