using FluentValidation;
using MovieWatch.Data.Pld;
using MovieWatch.Services.Services;

namespace MovieWatch.Api.Validators;

public class GetMoviesValidator : AbstractValidator<GetMoviesPld>
{
    public GetMoviesValidator(ITmdbService tmdbService)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .MustAsync(async (page, cancellationToken) =>
            {
                var (pageCount, _, _) = await tmdbService.GetDiscoverMoviesInfo();
                return page <= pageCount;
            })
            .WithMessage("Page number is too high");
    }
    
}