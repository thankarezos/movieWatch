using FluentValidation;
using MovieWatch.Data.Pld;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Validators;

public class AddFavoriteValidator : AbstractValidator<AddFavoritePld>
{

    public AddFavoriteValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.MovieIds)
            .NotNull()
            .WithMessage("MovieIds is required")
            .NotEmpty()
            .WithMessage("MovieIds is required")
            .Must(x => x.Count > 0)
            .WithMessage("MovieIds is required");
    }

}