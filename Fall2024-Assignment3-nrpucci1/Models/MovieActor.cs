using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fall2024_Assignment3_nrpucci1.Models
{
    //[PrimaryKey("MovieId", "ActorId")]
    public class MovieActor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a movie.")]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required(ErrorMessage = "Please select an actor.")]
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }
}
