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
using OpenAI;

namespace Fall2024_Assignment3_nrpucci1.Services
{
    public class AIService
    {
        private readonly AzureOpenAIClient _client;
        private readonly SentimentIntensityAnalyzer _sentimentAnalyzer;
        private readonly string _aiDeployment;
        private readonly bool _isApiConfigured; //for testing

        public AIService(IConfiguration configuration)
        {
            // Retrieve API credentials from configuration or secrets manager
            string apiKey = configuration["AzureOpenAI:ApiKey"];
            string apiEndpoint = configuration["AzureOpenAI:Endpoint"];
            _aiDeployment = configuration["AzureOpenAI:DeploymentName"]; // e.g., "gpt-35-turbo"

            _sentimentAnalyzer = new SentimentIntensityAnalyzer();

            if (!string.IsNullOrEmpty(apiEndpoint) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(_aiDeployment))
            {
                _client = new AzureOpenAIClient(new Uri(apiEndpoint), new ApiKeyCredential(apiKey));
                _isApiConfigured = true;
            }
            else
            {
                _isApiConfigured = false;
            }

            //var apiCredential = new ApiKeyCredential(apiKey);
            //_client = new AzureOpenAIClient(new Uri(apiEndpoint), apiCredential);
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

        public async Task<List<(string Tweet, double SentimentScore)>> GenerateActorTweetsAsync(string actorName)
        {
            var tweets = new List<(string Tweet, double SentimentScore)>();

            // Get ChatClient
            ChatClient chatClient = _client.GetChatClient(_aiDeployment);

            var messages = new ChatMessage[]
            {
        new SystemChatMessage($"You represent the Twitter social media platform. Generate an answer with a valid JSON formatted array of objects containing the tweet and username. The response should start with [."),
        new UserChatMessage($"Generate 20 tweets from a variety of users about the actor {actorName}.")
            };

            ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            JsonArray json = JsonNode.Parse(tweetsJsonString)!.AsArray();

            foreach (var tweetNode in json)
            {
                var tweetText = tweetNode["tweet"]?.ToString() ?? "";
                // Perform sentiment analysis
                var sentimentScore = _sentimentAnalyzer.PolarityScores(tweetText).Compound;
                tweets.Add((Tweet: tweetText, SentimentScore: sentimentScore));
            }

            return tweets;
        }

    }
}
