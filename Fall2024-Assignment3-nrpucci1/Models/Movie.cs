namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string ImdbLink { get; set; }
        public string Genre {  get; set; }
        public string Title { get; set; }

        public string YearOfRelease { get; set; }
        public string PosterURL { get; set; }

        public List<Actor> Actors { get; set; }
    }
}
