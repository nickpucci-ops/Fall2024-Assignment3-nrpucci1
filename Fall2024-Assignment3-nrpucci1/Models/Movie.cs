namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class Movie
    {
        public int Id { get; set; } //primary key
        public string ImdbLink { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string YearOfRelease { get; set; } = string.Empty;
        public string PosterURL { get; set; } = string.Empty;
        public ICollection<Actor> Actors { get; set; } = new List<Actor>(); //empty list for collection

        public Movie() { }
    }
}
