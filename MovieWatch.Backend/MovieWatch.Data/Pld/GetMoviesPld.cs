using System.ComponentModel.DataAnnotations;

namespace MovieWatch.Data.Pld;

public class GetMoviesPld
{
    [Required]
    public int Page { get; set; }
    public bool Verbose { get; set; } = false;
}