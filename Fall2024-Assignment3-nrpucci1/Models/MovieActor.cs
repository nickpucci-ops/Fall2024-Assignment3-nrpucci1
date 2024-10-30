using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fall2024_Assignment3_nrpucci1.Models;

namespace YourNamespace.Models
{
    public class MovieActor
    {
        [Key]
        public int Id { get; set; }

        // Foreign Key to Movie
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Foreign Key to Actor
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        public Actor Actor { get; set; }
    }
}
