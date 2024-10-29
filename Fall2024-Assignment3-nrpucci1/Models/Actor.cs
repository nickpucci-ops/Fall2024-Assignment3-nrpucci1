namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ImdbLink { get; set; }
        public string PhotoURL { get; set; }
        public List<Movie> Movies { get; set; }

    }
}
