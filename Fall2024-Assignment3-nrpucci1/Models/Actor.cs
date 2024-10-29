namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string ImdbLink { get; set; } = string.Empty;
        public string PhotoURL   { get; set; } = string.Empty;
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();

        public Actor() { }
    }
}
