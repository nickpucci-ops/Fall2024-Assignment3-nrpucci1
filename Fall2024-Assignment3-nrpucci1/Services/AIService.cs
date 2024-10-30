using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using VaderSharp2;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

namespace Fall2024_Assignment3_nrpucci1.Services
{
    public class AIService
    {
        private readonly AzureOpenAIClient _client;
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;
        private readonly string _aiDeployment;

        public AIService(IConfiguration configuration)
        {
            // Retrieve API credentials from configuration or secrets manager
            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string apiEndpoint = configuration["AzureOpenAI:Endpoint"];
            _aiDeployment = configuration["AzureOpenAI:DeploymentName"]; // e.g., "gpt-35-turbo"

            var apiCredential = new ApiKeyCredential(apiKey);
            _client = new AzureOpenAIClient(new Uri(apiEndpoint), apiCredential);
            _sentimentAnalyzer = new SentimentIntensityAnalyzer();
        }

        public async Task<List<(string Review, double SentimentScore)>> GenerateMovieReviewsAsync(string movieTitle, string releaseYear, string director)
        {
            var reviews = new List<(string Review, double SentimentScore)>();

            // Define personas
            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers", "loves fantasy" };

            // Get ChatClient
            ChatClient chatClient = _client.GetChatClient(_aiDeployment);

            foreach (string persona in personas)
            {
                var messages = new ChatMessage[]
                {
                    new SystemChatMessage($"You are a film reviewer and film critic who {persona}."),
                    new UserChatMessage($"How would you rate the movie {movieTitle} released in {releaseYear} directed by {director} out of 10 in less than 175 words?")
                };

                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 200,
                };

                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

                string reviewText = result.Value.Content[0].Text;

                // Perform sentiment analysis
                SentimentAnalysisResults sentiment = _sentimentAnalyzer.PolarityScores(reviewText);
                double sentimentScore = sentiment.Compound;

                reviews.Add((reviewText, sentimentScore));

                // Throttle requests to comply with rate limits
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            return reviews;
        }
    }
}
