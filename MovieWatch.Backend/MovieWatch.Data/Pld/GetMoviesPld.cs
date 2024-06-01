using System.ComponentModel.DataAnnotations;

namespace MovieWatch.Data.Pld;

public class GetMoviesPld
{
    [Required]
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TitleFilter { get; set; }
}