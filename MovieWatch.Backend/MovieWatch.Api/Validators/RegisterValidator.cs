using FluentValidation;
using MetroClimate.Data.Dtos.Payload;
using Microsoft.EntityFrameworkCore;
using MovieWatch.Data.Database;

namespace MovieWatch.Api.Validators;

public class RegisterValidator : AbstractValidator<RegisterPld>
{
    public RegisterValidator(MovieWatchDbContext dbContext)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MustAsync(async (username, token) =>
            {
                return !await dbContext.Users.AnyAsync(x => x.Username == username, cancellationToken: token);
            })
            .WithMessage("Username is already taken");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm Password is required")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .MustAsync(async (email, token) =>
            {
                return !await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken: token);
            })
            .WithMessage("Email is already taken");
    }
}