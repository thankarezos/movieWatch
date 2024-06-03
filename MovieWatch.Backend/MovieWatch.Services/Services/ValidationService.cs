using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MovieWatch.Data.Common;
using MovieWatch.Data.Constants;

namespace MovieWatch.Services.Services
{
    public interface IValidationService
    {
        ApiResponse<TData>? ValidatePld<TPayload, TData>(TPayload payload)
            where TPayload : class;

        Task<ApiResponse<TData>?> ValidatePldAsync<TPayload, TData>(TPayload payload)
            where TPayload : class;
    }
    
    public class ValidationService  : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public ApiResponse<TData>? ValidatePld<TPayload, TData>(TPayload payload)
            where TPayload : class
        {
            using var scope = _serviceProvider.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TPayload>>();
            var validationResult = validator.Validate(payload);

            return !validationResult.IsValid ? new ApiResponse<TData>(ErrorCode.BadRequest, "Validation error", validationResult) : null;
        }
        
        public async Task<ApiResponse<TData>?> ValidatePldAsync<TPayload, TData>(TPayload payload)
            where TPayload : class
        {
            using var scope = _serviceProvider.CreateScope();
            var validator = scope.ServiceProvider.GetRequiredService<IValidator<TPayload>>();
            var validationResult = await validator.ValidateAsync(payload);

            return !validationResult.IsValid ? new ApiResponse<TData>(ErrorCode.BadRequest, "Validation error", validationResult) : null;
        }
    }
}