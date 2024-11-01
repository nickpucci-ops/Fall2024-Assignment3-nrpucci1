using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_nrpucci1.Models;
public class Movie
{
    [Key]
    public int Id { get; set; } //primary key
    [Required]
    public string Title { get; set; }
    public string? ImdbLink { get; set; }
    public string? Genre { get; set; }
    [Required]
    public string YearOfRelease { get; set; }
    public string? PosterURL { get; set; }

    //public ICollection<MovieActor> MovieActor { get; set; } = new List<MovieActor>();
}
