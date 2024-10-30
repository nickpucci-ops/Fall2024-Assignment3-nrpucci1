using System.Collections.Generic;

namespace Fall2024_Assignment3_nrpucci1.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<(string Review, string Sentiment)> Reviews { get; set; }
        public string OverallSentiment { get; set; }
    }
}
