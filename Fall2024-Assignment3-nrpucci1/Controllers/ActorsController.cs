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
        public class ActorsController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly AIService _aiService;

            public ActorsController(ApplicationDbContext context, AIService aiService)
            {
                _context = context;
                _aiService = aiService;
            }

            public async Task<IActionResult> Index()
            {
                return View(await _context.Actors.ToListAsync());

                //var actors = await _context.Actors
                //    .Include(a => a.MovieActors)
                //        .ThenInclude(ma => ma.Movie)
                //    .ToListAsync();
                //return View(actors);
            }



            // GET: Actors/Details/5
            public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var actor = await _context.Actors
                    .FirstOrDefaultAsync(m => m.Id == id);

            // Temporarily comment out the MovieActors dependency
            // var movies = await _context.MovieActors
            //     .Include(cs => cs.Movie)
            //     .Where(cs => cs.ActorId == actor.Id)
            //     .Select(cs => cs.Movie)
            //     .ToListAsync();

            // Use an empty list as a placeholder for movies
                var movies = new List<Movie>();

            //var actor = await _context.Actors
            //    .Include(a => a.Movies)
            //    .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

                // Prepare the view model
                var viewModel = new ActorDetailsViewModel(actor, movies);
                //{
                //    Actor = actor,
                //    // We will populate Tweets and OverallSentiment below
                //};

                // Generate AI-generated tweets and perform sentiment analysis
                var tweetsWithScores = await _aiService.GenerateActorTweetsAsync(actor.Name);

                // Convert sentiment scores to labels
                var tweets = tweetsWithScores.Select(t => (
                    Tweet: t.Tweet,
                    Sentiment: t.SentimentScore > 0.05 ? "Positive" :
                               t.SentimentScore < -0.05 ? "Negative" : "Neutral"
                )).ToList();

                // Calculate overall sentiment
                int positiveCount = tweets.Count(t => t.Sentiment == "Positive");
                int negativeCount = tweets.Count(t => t.Sentiment == "Negative");

                string overallSentiment = "Neutral";
                if (positiveCount > negativeCount)
                    overallSentiment = "Positive";
                else if (negativeCount > positiveCount)
                    overallSentiment = "Negative";

                // Assign to the view model
                viewModel.Tweets = tweets;
                viewModel.OverallSentiment = overallSentiment;

                return View(viewModel);
            }


            // GET: Actors/Create
            public IActionResult Create()
            {
                return View();
            }

            // POST: Actors/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,ImdbLink,PhotoURL")] Actor actor)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(actor);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(actor);
            }

            // GET: Actors/Edit/5
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var actor = await _context.Actors.FindAsync(id);
                if (actor == null)
                {
                    return NotFound();
                }
                return View(actor);
            }

            // POST: Actors/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbLink,PhotoURL")] Actor actor)
            {
                if (id != actor.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(actor);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ActorExists(actor.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(actor);
            }

            // GET: Actors/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var actor = await _context.Actors
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (actor == null)
                {
                    return NotFound();
                }

                return View(actor);
            }

            // POST: Actors/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var actor = await _context.Actors.FindAsync(id);
                if (actor != null)
                {
                    _context.Actors.Remove(actor);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool ActorExists(int id)
            {
                return _context.Actors.Any(e => e.Id == id);
            }
        }
    }
