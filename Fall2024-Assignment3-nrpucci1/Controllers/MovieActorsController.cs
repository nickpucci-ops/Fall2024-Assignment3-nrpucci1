using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_nrpucci1.Data;
using Fall2024_Assignment3_nrpucci1.Models;

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
            var applicationdbContext = _context.MovieActors
                .Include(c => c.Movie)
                .Include(c => c.Actor);
            return View(await applicationdbContext.ToListAsync());
        }

        // GET: MovieActors/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title");
            ViewData["ActorId"] = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name");
            return View();
        }

        // POST: MovieActors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId,ActorId")] MovieActor movieActor)
        {
            bool alreadyExists = await _context.MovieActors
                .AnyAsync(cs => cs.MovieId == movieActor.MovieId && cs.ActorId == movieActor.ActorId);

            if (ModelState.IsValid && !alreadyExists)
            {
                _context.Add(movieActor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "This actor is already associated with the movie.");
            
            ViewData["MovieId"] = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title", movieActor.MovieId);
            ViewData["ActorId"] = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name", movieActor.ActorId);
            return View(movieActor);
        }

        // GET: MovieActors/Edit/5
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
            ViewData["MovieId"] = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title", movieActor.MovieId);
            ViewData["ActorId"] = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name", movieActor.ActorId);
            return View(movieActor);
        }

        // POST: MovieActors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId,ActorId")] MovieActor movieActor)
        {
            if (id != movieActor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieActor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieActorExists(movieActor.Id))
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
            ViewData["MovieId"] = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title", movieActor.MovieId);
            ViewData["ActorId"] = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name", movieActor.ActorId);
            return View(movieActor);
        }

        // GET: MovieActors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movieActor = await _context.MovieActors
                .Include(c => c.Movie)
                .Include(c => c.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActor == null)
            {
                return NotFound();
            }
            return View(movieActor);
        }

        // GET: MovieActors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movieActor = await _context.MovieActors
                .Include(c => c.Movie)
                .Include(c => c.Actor)
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
