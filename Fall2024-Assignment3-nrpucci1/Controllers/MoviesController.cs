using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_nrpucci1.Data;
using Fall2024_Assignment3_nrpucci1.Models;
using Fall2024_Assignment3_nrpucci1.Services;

namespace Fall2024_Assignment3_nrpucci1.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public MoviesController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // **GET: Movies**
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }


        // **GET: Movies/Details/5**
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var actors = await _context.MovieActor
                .Include(ma => ma.Actor)
                .Where(ma => ma.MovieId == movie.Id)
                .Select(ma => ma.Actor)
                .ToListAsync();

            //generate ai reviews
            var reviewsWithScores = await _aiService.GenerateMovieReviewsAsync(
                movie.Title,
                movie.YearOfRelease
            );

            // sentiment scores to labels
            var reviews = reviewsWithScores.Select(r => (
                r.Review,
                Sentiment: 
                r.SentimentScore >= 0.75 ? "Very Positive" 
                : r.SentimentScore < 0.75 && r.SentimentScore >= 0.25 ? "Positive"
                : r.SentimentScore < 0.25 && r.SentimentScore >= -0.25 ? "Neutral"
                : r.SentimentScore < -0.25 && r.SentimentScore >= -0.75 ? "Negative" 
                : r.SentimentScore < -0.75 ? "Very Negative"
                : "Neutral"
            )).ToList();

            //overall sentiment
            int positiveCount = reviews.Count(r => r.Sentiment == "Positive");
            int negativeCount = reviews.Count(r => r.Sentiment == "Negative");

            string overallSentiment = "Neutral";
            if (positiveCount > negativeCount)
                overallSentiment = "Positive";
            else if (negativeCount > positiveCount)
                overallSentiment = "Negative";

            var viewModel = new MovieDetailsViewModel(movie, actors);
            viewModel.OverallSentiment = overallSentiment;
            viewModel.Reviews = reviews;

            return View(viewModel);
        }

        // **GET: Movies/Create**
        public IActionResult Create()
        {
            return View();
        }

        // **POST: Movies/Create**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,YearOfRelease,ImdbLink,PosterURL")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // **GET: Movies/Edit/5**
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // **POST: Movies/Edit/5**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,YearOfRelease,ImdbLink,PosterURL")] Movie movie)
        {
            if (id != movie.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // **GET: Movies/Delete/5**
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // **POST: Movies/Delete/5**
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null) _context.Movie.Remove(movie);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
