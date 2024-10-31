using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class MovieActor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a movie.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid movie.")]
        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        [Required(ErrorMessage = "Please select an actor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid actor.")]
        public int ActorId { get; set; }

        public Actor Actor { get; set; }
    }
}
