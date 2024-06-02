using FluentValidation;
using MovieWatch.Data.Pld;

namespace MovieWatch.Api.Validators;

public class LoginValidator : AbstractValidator<LoginPld>
{
    public LoginValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
    
}