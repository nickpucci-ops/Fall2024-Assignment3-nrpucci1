﻿
namespace Fall2024_Assignment3_nrpucci1.Models;

public class MovieDetailsViewModel
{
    public Movie Movie { get; set; }
    public IEnumerable<Actor> Actors { get; set; }
    public string OverallSentiment { get; set; } = string.Empty;
    public List<(string Review, string Sentiment)> Reviews { get; set; }
    public MovieDetailsViewModel(Movie movie)
    {
        Movie = movie;
    }    
}