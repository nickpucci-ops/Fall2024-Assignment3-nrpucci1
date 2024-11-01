using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class MovieActorViewModel
    {
        public int MovieActorId { get; set; }

        [Required(ErrorMessage = "Please select a movie.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid movie.")] // 0 triggers selection error
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Please select an actor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid actor.")]
        public int ActorId { get; set; }

        // Lists for dropdowns
        public List<Movie> Movies { get; set; }
        public List<Actor> Actors { get; set; }

        public MovieActorViewModel()
        {
            Movies = new List<Movie>();
            Actors = new List<Actor>();
        }

        public SelectList MovieSelectList { get; set; }
        public SelectList ActorSelectList { get; set; }

    }
}
