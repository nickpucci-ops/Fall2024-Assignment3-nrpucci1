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
using YourNamespace.Models;


namespace Fall2024_Assignment3_nrpucci1.Controllers
{
    public class MovieActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MovieActors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MovieActors
                .Include(m => m.Actor)
                .Include(m => m.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MovieActors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActors
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // GET: MovieActor/Create
        public IActionResult Create()
        {
            var viewModel = new MovieActorViewModel
            {
                Movies = _context.Movies.ToList(),
                Actors = _context.Actors.ToList(),
                MovieActor = new MovieActor()
            };
            return View(viewModel);
        }

        // POST: MovieActor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieActorViewModel viewModel)
        {
            bool exists = _context.MovieActors
                .Any(ma => ma.MovieId == viewModel.MovieActor.MovieId && ma.ActorId == viewModel.MovieActor.ActorId);
            if (exists)
            {
                ModelState.AddModelError("", "This actor is already associated with the movie.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.MovieActor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Reload movies and actors lists if model state is invalid
            viewModel.Movies = _context.Movies.ToList();
            viewModel.Actors = _context.Actors.ToList();
            return View(viewModel);
        }


        // GET: MovieActor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movieActor = await _context.MovieActors.FindAsync(id);
            if (movieActor == null)
            {
                return NotFound();
            }
            var viewModel = new MovieActorViewModel
            {
                MovieActor = movieActor,
                Movies = _context.Movies.ToList(),
                Actors = _context.Actors.ToList()
            };
            return View(viewModel);
        }


        // POST: MovieActor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieActorViewModel viewModel)
        {
            if (id != viewModel.MovieActor.Id)
            {
                return NotFound();
            }
            bool exists = _context.MovieActors
                .Any(ma => ma.MovieId == viewModel.MovieActor.MovieId && ma.ActorId == viewModel.MovieActor.ActorId && ma.Id != id);

            if (exists)
            {
                ModelState.AddModelError("", "This actor is already associated with the movie.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.MovieActor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieActorExists(viewModel.MovieActor.Id))
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
            // Reload movies and actors lists if model state is invalid
            viewModel.Movies = _context.Movies.ToList();
            viewModel.Actors = _context.Actors.ToList();
            return View(viewModel);
        }


        // GET: MovieActors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActor = await _context.MovieActors
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }

            return View(movieActor);
        }

        // POST: MovieActors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActor = await _context.MovieActors.FindAsync(id);
            if (movieActor != null)
            {
                _context.MovieActors.Remove(movieActor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieActorExists(int id)
        {
            return _context.MovieActors.Any(e => e.Id == id);
        }
    }
}
