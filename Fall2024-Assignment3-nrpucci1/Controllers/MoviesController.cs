using System;
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

namespace Fall2024_Assignment3_nrpucci1.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIClient _openAiClient;
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;

            //OpenAI client (replace with actual endpoint and key from secrets)
            string endpoint = "https://your-openai-resource-name.openai.azure.com/";
            string apiKey = "Azure_OpenAI_Key";
            _openAiClient = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));

            //sentiment analyzer initialization
            _sentimentAnalyzer = new SentimentIntensityAnalyzer();
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Actors) //include related actors if applicable
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            //generates ai reviews
            var reviews = new List<(string Review, string Sentiment)>();
            for (int i = 0; i < 10; i++)
            {
                var response = await _openAiClient.GetChatCompletionsAsync("gpt-35-turbo", new ChatCompletionsOptions
                {
                    Messages = new List<ChatMessage>
                    {
                        new ChatMessage("user", $"Write a review for the movie '{movie.Title}'.")
                    }
                });

                var reviewText = response.Value.Choices.First().Message.Content;

                //analyze the sentiment of given review
                var sentimentScore = _sentimentAnalyzer.PolarityScores(reviewText).Compound;
                var sentiment = sentimentScore > 0 ? "Positive" : "Negative";

                reviews.Add((reviewText, sentiment));
            }

            //calculate the overall sentiment of the reviews
            var overallSentiment = reviews.Count(r => r.Sentiment == "Positive") > 5 ? "Positive" : "Negative";

            //prepare view model
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
