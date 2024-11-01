namespace Fall2024_Assignment3_nrpucci1.Models;
public class ActorDetailsViewModel
{
    public Actor Actor { get; set; }
    public IEnumerable<Movie>? Movies { get; set; }
    public string OverallSentiment { get; set; } = string.Empty;
    public List<(string Tweet, string Sentiment)>? Tweets { get; set; } = new List<(string, string)>();
    public ActorDetailsViewModel(Actor actor, IEnumerable<Movie> movies)
    {
        Actor = actor;
        Movies = movies;
    }
}
