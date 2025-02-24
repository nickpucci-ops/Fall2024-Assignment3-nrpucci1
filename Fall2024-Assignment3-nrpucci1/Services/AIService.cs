﻿using System;
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
        private readonly bool _isApiConfigured; //for testing
        private readonly string _aiDeployment;

        public AIService(string key , string endpoint, string deployment)
        {
            string apiKey = key;
            string apiEndpoint = endpoint;
            _aiDeployment = deployment;

            //check for environment variables first
            //string apiKey = Environment.GetEnvironmentVariable("AzureOpenAI:ApiKeySecret") ?? configuration["AzureOpenAI:ApiKeySecret"];
            //string apiEndpoint = Environment.GetEnvironmentVariable("AzureOpenAI:EndpointSecret") ?? configuration["AzureOpenAI:EndpointSecret"];
            //_aiDeployment = Environment.GetEnvironmentVariable("AzureOpenAI:DeploymentNameSecret") ?? configuration["AzureOpenAI:DeploymentNameSecret"];

            //string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            //string apiEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            //_aiDeployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME");


            _sentimentAnalyzer = new SentimentIntensityAnalyzer();

            var apiCredential = new ApiKeyCredential(apiKey);
            _client = new AzureOpenAIClient(new Uri(apiEndpoint), apiCredential);   
            //if (!string.IsNullOrEmpty(apiEndpoint) && !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(_aiDeployment))
            //{
            //    _isApiConfigured = true;
            //}
            //else
            //{
            //    _isApiConfigured = false;
            //}
        }

        public async Task<List<(string Review, double SentimentScore)>> GenerateMovieReviewsAsync(string movieTitle, string releaseYear)
        {
            var reviews = new List<(string Review, double SentimentScore)>();

            //if (!_isApiConfigured)
            //{
            //    // Return mock data
            //    reviews = new List<(string Review, double SentimentScore)>
            //    {
            //        ("An outstanding performance and a captivating story!", 0.85),
            //        ("The movie was okay, but could have been better.", 0.0),
            //        ("I didn't enjoy the film; it was quite disappointing.", -0.6),
            //    };
            //    return reviews;
            //}

            string[] personas = { "is harsh", "loves romance", "loves comedy", "loves thrillers",
                "loves fantasy", "loves sci-fi", "absolutely hates movies", 
                "thinks Mr. Maclane is an awesome professor", "loves action but does not love fantasy or sci-fi" };

            ChatClient chatClient = _client.GetChatClient(_aiDeployment);

            foreach (string persona in personas)
            {
                var messages = new ChatMessage[]
                {
                    new SystemChatMessage($"You are a film reviewer and film critic who {persona}."),
                    new UserChatMessage($"How would you rate the movie {movieTitle} released in {releaseYear} out of 10 in less than 100 words?")
                };

                var chatCompletionOptions = new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 120, //buffer to stay within rate limit
                };

                ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

                string reviewText = result.Value.Content[0].Text;

                SentimentAnalysisResults sentiment = _sentimentAnalyzer.PolarityScores(reviewText);
                double sentimentScore = sentiment.Compound;

                reviews.Add((reviewText, sentimentScore));

                await Task.Delay(500); //0.5second delay stays within rate limit
            }

            return reviews;
        }

        public async Task<List<(string Tweet, double SentimentScore)>> GenerateActorTweetsAsync(string actorName)
        {
            var tweets = new List<(string Tweet, double SentimentScore)>();
            //if (!_isApiConfigured)
            //{
            //    // Return mock data
            //    tweets = new List<(string Tweet, double SentimentScore)>
            //    {
            //        ($"Just watched a movie starring {actorName}, and it was fantastic!", 0.9),
            //        ($"{actorName} is so overrated.", -0.7),
            //        ($"{actorName}'s latest performance was absolutely brilliant.", 0.8),
            //    };
            //    return tweets;
            //}

            ChatClient chatClient = _client.GetChatClient(_aiDeployment);

            var messages = new ChatMessage[]
            {
                new SystemChatMessage($"You represent the Twitter social media platform. " +
                $"Generate an answer with a valid JSON formatted array of objects containing the tweet and username. " +
                $"The response should start with [. Only respond with JSON. Do not reply to the prompt otherwise."),
                new UserChatMessage($"Generate 20 tweets from a variety of users about the actor {actorName}.")
            };

            ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages);

            string tweetsJsonString = result.Value.Content.FirstOrDefault()?.Text ?? "[]";
            JsonArray json = JsonNode.Parse(tweetsJsonString)!.AsArray();

            foreach (var tweetNode in json)
            {
                var tweetText = tweetNode["tweet"]?.ToString() ?? "";
                var sentimentScore = _sentimentAnalyzer.PolarityScores(tweetText).Compound;
                tweets.Add((Tweet: tweetText, SentimentScore: sentimentScore));
            }

            return tweets;
        }

    }
}
