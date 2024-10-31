using Fall2024_Assignment3_nrpucci1.Models;

namespace YourNamespace.Models
{
    public class MovieActorViewModel
    {
        public MovieActor MovieActor { get; set; }

        // Lists for dropdowns
        public List<Movie> Movies { get; set; }
        public List<Actor> Actors { get; set; }

        public MovieActorViewModel()
        {
            Movies = new List<Movie>();
            Actors = new List<Actor>();
        }
    }
}
