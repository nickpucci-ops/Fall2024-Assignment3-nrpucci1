using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_nrpucci1.Data;
using Fall2024_Assignment3_nrpucci1.Models;
using VaderSharp2; //used for the sentiment analysis
using Fall2024_Assignment3_nrpucci1.Services;


namespace Fall2024_Assignment3_nrpucci1.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AzureOpenAIClient _openAiClient;
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;
        private readonly AIService _aiService;

        public MoviesController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;

            //OpenAI client (replace with actual endpoint and key from secrets)
            string endpoint = "https://your-openai-resource-name.openai.azure.com/";
            string apiKey = "Azure_OpenAI_Key";
            _openAiClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));


            //sentiment analyzer initialization
            _sentimentAnalyzer = new SentimentIntensityAnalyzer();
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            // Generate AI reviews using the AIService
            var reviewsWithScores = await _aiService.GenerateMovieReviewsAsync(
                movie.Title,
                movie.YearOfRelease.ToString(),
                "Director Name" // Replace with actual director if available
            );

            // Convert sentiment scores to labels
            var reviews = reviewsWithScores.Select(r => (
                Review: r.Review,
                Sentiment: r.SentimentScore > 0.05 ? "Positive" : r.SentimentScore < -0.05 ? "Negative" : "Neutral"
            )).ToList();

            // Calculate overall sentiment
            int positiveCount = reviews.Count(r => r.Sentiment == "Positive");
            int negativeCount = reviews.Count(r => r.Sentiment == "Negative");

            string overallSentiment = "Neutral";
            if (positiveCount > negativeCount)
                overallSentiment = "Positive";
            else if (negativeCount > positiveCount)
                overallSentiment = "Negative";

            // Prepare the view model
            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                OverallSentiment = overallSentiment
            };

            return View(viewModel);
        }
    }
}
