namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }
        public List<(string Tweet, string Sentiment)> Tweets { get; set; } = new List<(string, string)>();
        public string OverallSentiment { get; set; } = string.Empty;
    }
}
