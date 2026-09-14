using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication11.Data;
using WebApplication11.Models;
using WebApplication11.Services;

namespace WebApplication11.Controllers
{

    public class HomeController : Controller
    {
        private readonly IOmdbService _omdbService;
        private readonly AppDbContext _context;
        
        public HomeController(IOmdbService omdbService, AppDbContext context)
        {
            _omdbService = omdbService ?? throw new ArgumentNullException(nameof(omdbService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new MovieViewModel
            {
                RecentMovies = await GetRecentMoviesAsync()
            };
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Favorites()
        {
            var favoriteMovies = await _context.Movies
                .Where(m => m.IsFavorite)
                .OrderByDescending(m => m.ImdbRating)
                .ToListAsync();

            return View(favoriteMovies);
        }

        [HttpPost]
        public async Task<IActionResult> Index(MovieViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SearchQuery))
            {
                model.ErrorMessage = "Будь ласка, введіть назву фільму.";
                model.RecentMovies = await GetRecentMoviesAsync();
                return View(model);
            }

            var apiResult = await _omdbService.GetMovieByTitleAsync(model.SearchQuery);

            if (apiResult != null && apiResult.Response == "True")
            {
                var existingMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.Title.ToLower() == apiResult.Title.ToLower());

                if (existingMovie != null)
                {
                    existingMovie.SearchedAt = DateTime.UtcNow;
                    _context.Movies.Update(existingMovie);
                    model.CurrentMovie = existingMovie;
                }
                else
                {
                    var newEntity = new MovieEntity
                    {
                        Title = apiResult.Title,
                        Year = apiResult.Year,
                        Poster = apiResult.Poster,
                        ImdbRating = apiResult.ImdbRating,
                        Runtime = apiResult.Runtime,
                        Director = apiResult.Director,
                        Actors = apiResult.Actors,
                        Plot = apiResult.Plot,
                        SearchedAt = DateTime.UtcNow
                    };

                    _context.Movies.Add(newEntity);
                    model.CurrentMovie = newEntity;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                model.ErrorMessage = apiResult?.Error == "Movie not found!"
                    ? "Фільм не знайдено."
                    : apiResult?.Error ?? "Сталася помилка під час пошуку.";
            }

            model.RecentMovies = await GetRecentMoviesAsync();
            return View(model);
        }
        
        private Task<List<MovieEntity>> GetRecentMoviesAsync()
        {
            return _context.Movies
                .OrderByDescending(m => m.SearchedAt)
                .Take(5)
                .ToListAsync();
        }
    }
}