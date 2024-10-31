using Fall2024_Assignment3_nrpucci1.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class MovieActorViewModel
    {
        public MovieActor MovieActor { get; set; }

        // Lists for dropdowns
        public List<Movie> Movies { get; set; }
        public List<Actor> Actors { get; set; }

        // SelectLists for dropdowns
        public SelectList MovieSelectList { get; set; }
        public SelectList ActorSelectList { get; set; }

        public MovieActorViewModel()
        {
            Movies = new List<Movie>();
            Actors = new List<Actor>();
        }
    }
}
