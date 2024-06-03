using System.ComponentModel.DataAnnotations;

namespace MovieWatch.Data.Pld;

public class AddFavoritePld
{
    [Required]
    public required List<int> MovieIds { get; set; }
    
}